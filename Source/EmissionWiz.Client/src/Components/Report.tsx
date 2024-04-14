import * as React from 'react';
import { observable } from 'mobx';
import { observer } from 'mobx-react';

import DocumentTitle from 'react-document-title';

export type ReportFilterProps<TModel extends {}> = {
    configGroup: string;
    onFilterChanged: (filter: TModel) => void;
};

type ReportProps = {
    title?: string;
    classNames?: {
        content?: string;
        header?: string;
        body?: string;
    }
    selfTitle?: boolean;
    selfContent?: boolean;
    showOnlyHeaderElement?: boolean;
    headerElement?: JSX.Element | null;
    isFlexContainer?: boolean;
    children?: React.ReactNode;
};

@observer
export class Report extends React.Component<ReportProps> {
    @observable private accessor _hasError: boolean;
    @observable private accessor _error: Error | null;
    @observable private accessor _componentStack: string | null;

    componentDidCatch(error: Error, errorInfo: React.ErrorInfo) {
        this._hasError = true;
        this._error = error;
        this._componentStack = errorInfo.componentStack ?? null;
    }

    render() {
        const { title, classNames, isFlexContainer, selfTitle, selfContent, showOnlyHeaderElement, headerElement } = this.props;
        if (selfContent && !this._hasError) {
            return this.props.children;
        } else {
            const contentClass = `content ${isFlexContainer ? 'content-flex' : ''} ${classNames?.content ?? ''}`;
            const contentBodyClass = `content-body ${classNames?.body || ''}`;
            return (
                <div className={contentClass}>
                    {title && <DocumentTitle title={title} />}
                    <div className={contentBodyClass}>
                        {!selfTitle && (
                            <h2 className="clearfix">
                                {title}
                                {headerElement || ''}
                            </h2>
                        )}
                        {showOnlyHeaderElement && selfTitle && <h2 className={classNames?.header || ''}>{headerElement || ''}</h2>}
                        {this._hasError && this.renderError()}
                        {!this._hasError && this.props.children}
                    </div>
                </div>
            );
        }
    }

    renderError() {
        const message: string = this._error ? this._error.message : 'Error';
        return (
            <div className="error-container">
                <h3>Something went wrong</h3>
                <p>Please contact support.</p>
                <div className="stack-block">
                    <h3>{message}</h3>
                    The above error occurred in:
                    {this._componentStack}
                </div>
            </div>
        );
    }
}
