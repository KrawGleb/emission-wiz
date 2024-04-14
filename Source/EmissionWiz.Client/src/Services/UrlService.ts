export default class UrlService {
    public static getCurrentUrl(): string {
        return window.location.href;
    }

    public static getParam(key: string): string | null {
        const params = new URLSearchParams(window.location.search);
        return params.get(key);
    }

    public static openInNewWindow(url: string, param?: string, name?: string, specs?: string) {
        let params: string = '';

        if (param) {
            params = window.location.search ? window.location.search + '&' + param : '?' + param;
        }

        window.open(url + params, name, specs);
    }

    public static clearParams() {
        window.history.replaceState({}, document.title, window.location.pathname);
    }

    public static updateParams(key: string, value: string | null) {
        const path = window.location.pathname;
        const params = new URLSearchParams(window.location.search);
        if (value !== null) {
            params.set(key, value);
        } else {
            params.delete(key);
        }
        window.history.replaceState({}, document.title, path + '?' + params.toString());
    }
    
    public static getParams(params: [string, string | null | undefined][]) {
        const values = params.map(p => {
            const [name, value] = p;
            if (!value){
                return null;
            }

            return `${name}=${encodeURIComponent(value)}`;
        }).filter(x => !!x);

        if (values.length){
            return '?' + values.join('&');
        }

        return '';
    }
}
