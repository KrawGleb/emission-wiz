import React from "react";
import { observer } from "mobx-react";
import { action, computed, observable } from "mobx";
import { pdfjs, Document, Page } from 'react-pdf';
import Extensions from "../Helpers/Extensions";
import { Typography } from 'antd';
import { DocumentCallback } from "react-pdf/dist/cjs/shared/types";

const { Text } = Typography;

pdfjs.GlobalWorkerOptions.workerSrc = new URL(
    'pdfjs-dist/build/pdf.worker.min.js',
    import.meta.url,
).toString();

type PdfViewerProps = {
    pdfData: Blob | null;
    onLoadError?: () => void;
    onRenderPages?: (index: number, renderCallback: (index: number) => JSX.Element) => void;
};

@observer
export default class PdfViewer extends React.Component<PdfViewerProps> {
    @observable private accessor _pagesNumber: number = 0;
    @observable private accessor _pageWidth: number = 0;
    @observable private accessor _loadingError: Error | null = null;
    private _pdfWrapper: React.RefObject<HTMLDivElement> = React.createRef<HTMLDivElement>();

    componentDidMount() {
        this._setSize();
        window.addEventListener('resize', this._onWindowResize);
    }

    componentDidUpdate(prevProps: PdfViewerProps) {
        if (!prevProps.pdfData || !this.props.pdfData || prevProps.pdfData.size != this.props.pdfData.size) {
            this._pagesNumber = 0;
        }
    }

    componentWillUnmount() {
        window.removeEventListener('resize', this._onWindowResize);
    }

    private _onWindowResize = () => {
        Extensions.executeTimeout(this._setSize, 500, this);
    };

    private _setSize() {
        if (!this._pdfWrapper.current) return;
        this._pageWidth = this._pdfWrapper.current.getBoundingClientRect().width;

        const scrollbarWidthPx = 19;
        const containerPaddingPx = 3;
        const overflowWidth = scrollbarWidthPx + containerPaddingPx;
        this._pageWidth -= overflowWidth;
    }

    render() {
        return (
            <div className={'pdf-container'} ref={this._pdfWrapper}>
                <Document
                    file={this.props.pdfData ? new File([this.props.pdfData], "pdf") : undefined}
                    noData={this.renderNoData()}
                    loading={this.renderNoData()}
                    onLoadSuccess={this._onLoadSuccess}
                    onSourceError={this._onLoadError}
                    onLoadError={this._onLoadError}
                >
                    {Array.from(
                        new Array(this._pagesNumber),
                        (_, index) => {
                            return this.renderPage(index);
                        },
                    )}
                </Document>
                {this._loadingError && <Text type="danger">{this._loadingError.message}</Text>}
            </div>
        );
    }

    renderPage = (index: number) => {
        return (
            <Page
                key={`page_${index + 1}`}
                pageNumber={index + 1}
                width={this._pageWidth || 400}
                renderMode="canvas"
                renderTextLayer={false}
                renderAnnotationLayer={false}
            />
        );
    };

    renderNoData() {
        return (
            <div className="alert alert-success" role="alert">
                Loading preview...
            </div>
        );
    }

    @action.bound
    private async _onLoadSuccess(doc: DocumentCallback) {
        this._pagesNumber = doc.numPages;
        this._loadingError = null;
    }

    @action.bound
    private async _onLoadError(error: Error) {
        this._pagesNumber = 0;
        this._loadingError = error;
        this.props.onLoadError?.();
    }
}