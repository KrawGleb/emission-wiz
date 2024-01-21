import ApiService, { IAjaxOptions } from './ApiService';

export class DownloadService {
    public async downloadFile<T>(url: string, params?: T, sourceFileName?: string, options?: IAjaxOptions) {
        const { data, headers } = await ApiService.getTypedData<Buffer>(url, params, { responseType: 'blob', ...options });

        let fileName = sourceFileName;
        if (!fileName) {
            fileName = '';
            const disposition = headers['content-disposition'];
            if (disposition && disposition.indexOf('attachment') !== -1) {
                const filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
                const matches = filenameRegex.exec(disposition);
                if (matches && matches[1]) {
                    fileName = matches[1].replace(/['"]/g, '');
                }
            }
        }

        const blob = new Blob([data]);
        await this.downloadBlob(blob, fileName);
    }

    public async downloadBlob(blob: Blob, fileName: string) {
        const objectUrl = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = objectUrl;
        link.setAttribute('download', fileName);
        document.body.appendChild(link);
        await link.click();
    }
}

export const downloadService = new DownloadService();