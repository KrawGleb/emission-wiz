/* eslint-disable @typescript-eslint/no-explicit-any */
import { emitter } from './EventEmitter';
import Axios, { AxiosInstance, AxiosPromise, AxiosRequestConfig, ResponseType, AxiosResponse, AxiosError, CancelToken, CancelTokenSource } from 'axios';
import { globalAjaxLoaderStore } from '../Stores/GlobalAjaxLoaderStore';

import DateTimeService from '../Services/DateTimeService';
import { PromiseCompletion } from '../Classes/PromiseCompletion';

export type ApiHeaders = {
    [key: string]: string | number;
};

export interface IAjaxOptions {
    responseType?: ResponseType;
    hideModalLoader?: boolean;
    requestKey?: string;
    transformDateTime?: boolean;
    operationName?: string;
    noDateTransform?: boolean;
    suppressErrorHandling?: boolean;
    completion?: PromiseCompletion | (PromiseCompletion | undefined)[];
    cancelDuplicatedRequests?: boolean;
    cancellationToken?: CancelToken;
    customApiHeader?: ApiHeaders;
    timeout?: number;
}

type AxiosMethodDefinition = (url: string, data?: any, config?: AxiosRequestConfig) => AxiosPromise;
type AxiosMethodChooser = (instance: AxiosInstance) => AxiosMethodDefinition;

export type Response = AxiosResponse;
export type ResponseError = AxiosError;
export type ResponseInterceptor = {
    response: (value: Response) => Response | Promise<Response>;
    error: (err: ResponseError) => ResponseError;
};

export enum AxiosMethod {
    GET = 'GET',
    DELETE = 'DELETE',
    HEAD = 'HEAD',
    OPTIONS = 'OPTIONS',
    POST = 'POST',
    PUT = 'PUT',
    PATCH = 'PATCH'
}

type CreateHeaderHandler = () => { key: ApiHeadersKeys, value: string } | undefined;

export enum ApiHeadersKeys {
    CacheControl = 'Cache-Control',
    Authorization = 'Authorization',
    AppIsHistorized = 'App-Is-Historized',
    AppHistorizedDate = 'App-Historized-Date',
    AppIsChangeLogDisabled = 'App-Is-Change-Log-Disabled',
    AppOperationName = 'App-Operation-Name',
    AppSignalRConnectionId = 'App-SignalR-ConnectionId',
    AppImpersonationPersonId = 'App-Impersonation-PersonId',
    AppRefreshData = 'App-Refresh-Data',
    AppViewPersonId = 'App-View-PersonId',
    AppImpersonationRoles = 'App-Impersonation-Roles',
    AppImpersonateAsPrincipal = 'App-Impersonate-As-Principal',
    AllowServerTrace = 'X-Allow-Server-Trace'
}

const startupTime = DateTimeService.now();

export default class ApiService {
    private static _responseInterceptors: ResponseInterceptor[] = [];
    private static _cancelTokenCollection: Map<string, CancelTokenSource> = new Map();
    private static _handlerList: CreateHeaderHandler[] = [];

    public static addResponseInterceptor(interceptor: ResponseInterceptor) {
        ApiService._responseInterceptors.push(interceptor);
    }

    private static _addAndChechCancelationToken(tokenKey: string, source: CancelTokenSource) {
        const duplicateToken = ApiService._cancelTokenCollection.get(tokenKey);
        if (duplicateToken) {
            duplicateToken.cancel();
        }
        ApiService._cancelTokenCollection.set(tokenKey, source);
    }

    private static _callMethod<TResponse>(methodChooser: AxiosMethodChooser, url: string, data?: unknown, options?: IAjaxOptions): AxiosPromise<TResponse> {
        const responseType = (options && options.responseType) || 'json';

        const requestKey = options?.requestKey ? options.requestKey : this._getAbsoluteUrl(url);

        let cancelToken = options?.cancellationToken;
        if (options?.cancelDuplicatedRequests) {
            const source = Axios.CancelToken.source();
            cancelToken = source.token;
            ApiService._addAndChechCancelationToken(requestKey, source);
        }
        const instance = ApiService._getInstance(responseType, options, cancelToken);

        const result = methodChooser(instance)(this._getAbsoluteUrl(url), data);
        const requestTimestamp = DateTimeService.currentTime();
        const isHandleError = !(options && options.suppressErrorHandling);
        globalAjaxLoaderStore.showAjaxSpinner(requestTimestamp);
        if (options && options.completion) {
            if (Array.isArray(options.completion)) {
                options.completion.forEach((c) => c?.subscribe(result));
            } else {
                options.completion.subscribe(result);
            }
        }
        result.catch((error: AxiosError) => {
            if (isHandleError || error.response?.status.toString().charAt(0) === '5') this.handleError(url, error);
            globalAjaxLoaderStore.hideAjaxSpinner(requestTimestamp);
        });
        void result.then(() => {
            globalAjaxLoaderStore.hideAjaxSpinner(requestTimestamp);
        });
        return result;
    }

    public static getRequestUrl(baseUrl: string, operation: unknown) {
        console.log(baseUrl, operation);
    }

    public static sendRequest<TResponse>(method: AxiosMethod, url: string, data?: unknown, options?: IAjaxOptions): AxiosPromise<TResponse> {
        switch (method) {
            case AxiosMethod.POST:
                return ApiService.postTypedData<TResponse>(url, data, options);
            case AxiosMethod.GET:
                return ApiService.getTypedData<TResponse>(url, data, options);
            case AxiosMethod.PUT:
                return ApiService.putTypedData<TResponse>(url, data, options);
            case AxiosMethod.DELETE:
                return ApiService.deleteData<TResponse>(url, data, options);
            case AxiosMethod.PATCH:
                return ApiService.patchTypedData<TResponse>(url, data, options);
            default:
                return Promise.reject();
        }
    }

    //obsolete
    public static getData(url: string, getData?: unknown, options?: IAjaxOptions): AxiosPromise {
        return this.getTypedData<any>(url, getData, options);
    }

    public static get<TResponse>(url: string, getData?: unknown, options?: IAjaxOptions): AxiosPromise<TResponse> {
        return ApiService._callMethod<TResponse>((instance: AxiosInstance) => instance.get, url, { params: getData }, options);
    }

    //obsolete
    public static getTypedData<TResponse>(url: string, getData?: unknown, options?: IAjaxOptions): AxiosPromise<TResponse> {
        return ApiService._callMethod<TResponse>((instance: AxiosInstance) => instance.get, url, { params: getData }, options);
    }

    public static putData(url: string, putData?: unknown, options?: IAjaxOptions): AxiosPromise {
        return ApiService._callMethod((instance: AxiosInstance) => instance.put, url, putData, options);
    }

    public static putTypedData<TResponse>(url: string, putData?: unknown, options?: IAjaxOptions): AxiosPromise<TResponse> {
        return ApiService._callMethod((instance: AxiosInstance) => instance.put, url, putData, options);
    }

    public static postData(url: string, postData?: unknown, options?: IAjaxOptions): AxiosPromise {
        return ApiService._callMethod((instance: AxiosInstance) => instance.post, url, postData, options);
    }

    public static postTypedData<TResponse>(url: string, postData?: unknown, options?: IAjaxOptions): AxiosPromise<TResponse> {
        return ApiService._callMethod<TResponse>((instance: AxiosInstance) => instance.post, url, postData, options);
    }

    public static patchTypedData<TResponse>(url: string, patchData?: unknown, options?: IAjaxOptions): AxiosPromise<TResponse> {
        return ApiService._callMethod<TResponse>((instance: AxiosInstance) => instance.patch, url, patchData, options);
    }

    public static deleteData<TResponse>(url: string, deleteData?: unknown, options?: IAjaxOptions): AxiosPromise<TResponse> {
        return ApiService._callMethod<TResponse>((instance: AxiosInstance) => instance.delete, url, { params: deleteData }, options);
    }

    public static handleError(url: string, error: any) {
        if (Axios.isCancel(error)) {
            console.log(`Request canceled: ${url}\nMessage: ${error.message}`);
            return;
        }
        if (error && error.response && error.response.status === 409) {
            return;
        }
        if (error && error.response && error.response.status === 503) {
            globalAjaxLoaderStore.showAppLoader('Service is unavailable at the moment. Try later.');
            return;
        }
        errorHandleService.showError(url, error);
    }

    public static toQueryString(params: { [key: string]: string }) {
        if (typeof params !== 'object') return '';
        return `?${Object.keys(params)
            .map((k) => `${k}=${params[k]}`)
            .join('&')}`;
    }

    static typeCheck = (el: any, isDate?: boolean) => {
        if (!el) return el;
        let typeEl = el;
        switch (typeof el) {
            case 'string':
                typeEl = ApiService.strCheck(el, isDate);
                break;
            case 'object':
                typeEl = Array.isArray(el) ? ApiService.arrCheck(el) : ApiService.objCheck(el);
                break;
        }
        return typeEl;
    };

    private static strCheck = (str: string, isDate?: boolean) => {
        if (isDate && DateTimeService.ISO_8601_date.test(str)) return DateTimeService.fromString(str);
        return str;
    };

    private static arrCheck = (array: any) => {
        return array.map((el: any) => {
            return ApiService.typeCheck(el);
        });
    };

    private static objCheck = (obj: any) => {
        Object.keys(obj).forEach((key) => {
            obj[key] = ApiService.typeCheck(obj[key], key.indexOf('date') === 0 
                || key.indexOf('Date') !== -1 
                || key.indexOf('timestamp') !== -1 
                || key.endsWith('Timestamp') 
                || key.endsWith('Utc') 
                || key.endsWith('Lt'));
        });
        return obj;
    };

    private static _getInstance(responseType: ResponseType = 'json', options?: IAjaxOptions, cancelToken?: CancelToken): AxiosInstance {
        const headers = ApiService._createRequestHeaders(options);
        const transformResponse = (data: unknown, headers: { [key: string]: string }) => {
            if ((options && options.noDateTransform) || options?.responseType === 'text') return data;

            let result = data;
            if (typeof result === 'string' && headers['content-type']?.startsWith('application/json')) {
                result = JSON.parse(result);
            }

            if (typeof result === 'object') {
                result = ApiService.typeCheck(result);
            }

            return result;
        };

        const axiosConfig: AxiosRequestConfig = {
            responseType: responseType,
            headers: headers,
            transformResponse: transformResponse,
            cancelToken: cancelToken,
            timeout: options?.timeout
        };

        const axiosInstance = Axios.create(axiosConfig);

        this._useInterceptors(axiosInstance, responseType);
        ApiService._responseInterceptors.forEach((x) => axiosInstance.interceptors.response.use(x.response, x.error));

        return axiosInstance;
    }

    private static _useInterceptors(axiosInstance: AxiosInstance, responseType: string) {
        axiosInstance.interceptors.response.use(
            (res) => {
                const contentType = res.headers['content-type'];
                if (contentType && contentType.indexOf('text/html') > -1 && responseType === 'json') {
                    throw new Error('API call to ' + res.config.url + ' returned not expected content type: ' + contentType);
                }
                return res;
            },
            (err) => {
                if (err.response && err.response.status === 401) {
                    return new Promise(() => {
                        // TODO: login
                    });
                }
                if (err.response && err.response.status === 403) {
                    emitter.emit('notify-error-403', err);
                    return Promise.reject(err);
                }
                throw err;
            }
        );
    }

    public static subscribe(callback: CreateHeaderHandler) {
        if (this._handlerList.indexOf(callback) !== -1) {
            throw new Error('Callback is already registered!');
        }
        this._handlerList.push(callback);
    }

    public static unsubscribe(callback: CreateHeaderHandler) {
        const callbackIndex = this._handlerList.indexOf(callback);
        if (callbackIndex === -1) throw new Error('Callback is not registered!');
        this._handlerList.splice(callbackIndex, 1);
    }

    private static _getHeadersFromSubscriptions() {
        const headers: ApiHeaders = {};
        this._handlerList.forEach(callback => {
            const header = callback();
            if (header) {
                headers[header.key] = headers[header.key] || header.value;
            }
        });

        return headers;
    }

    private static _createRequestHeaders(options?: IAjaxOptions): ApiHeaders {
        const headers: ApiHeaders = (options && options.customApiHeader) || this._getHeadersFromSubscriptions();
        
        headers[ApiHeadersKeys.CacheControl] = headers[ApiHeadersKeys.CacheControl] || 'no-cache';

        if (window.performance) {
            const performance = window.performance.getEntriesByType("navigation");
            if (performance && performance[0]) {
                const type = (performance[0] as unknown as { type: string }).type;

                //hint to the server to try refreshng data in case user reloaded page
                if (String(type) === 'reload' && DateTimeService.diffSeconds(DateTimeService.now(), startupTime) < 10) {
                    headers[ApiHeadersKeys.AppRefreshData] = headers[ApiHeadersKeys.AppRefreshData] || 'true';
                }
            }
        }

        return headers;
    }

    private static _getAbsoluteUrl(url: string): string {
        let relariveUrl = url;
        if (relariveUrl && relariveUrl[0] !== '/') relariveUrl = '/' + url;
        if (!relariveUrl.startsWith('/api')) relariveUrl = '/api' + url;
        return location.origin + relariveUrl;
    }

    public static getDynamicUrl(url: string, params?: any & {}): string {
        let _url = url;
        for (const key in params) {
            if (params.hasOwnProperty(key)) {
                _url = _url.replace(`{${key}}`, encodeURIComponent(params[key] as string));
            }
        }
        return _url;
    }
}

class ErrorHandleService {
    private _lastError: { message: string; time: number };

    showError(_: string, error: any) {
        let errorMessage = '';
        const currentTime = new Date().getTime();
        console.log('--error AJAX: ', error);

        // TODO: Notify about error
        if (!this._lastError || this._lastError.message !== error.message || currentTime - this._lastError.time > 1000) {

            if (!error?.response) {
                const defaultErrorMessage = 'Something went wrong... Try again in a little bit.';
                errorMessage = error.message || defaultErrorMessage;
                // NotificationHandler.showError(errorMessage, '--error AJAX');
            }

            if (error.response && error.response.status !== 403) {
                if (error && error.response && (error.response.status === 400 || error.response.status === 500)) {
                    // void modalService.showError(error);
                } else {
                    // NotificationHandler.showError(error.message.toString(), '--error AJAX');
                }
                errorMessage = error.message;
            }

            this._lastError = { message: errorMessage, time: currentTime };
        }
    }
}

const errorHandleService = new ErrorHandleService();
