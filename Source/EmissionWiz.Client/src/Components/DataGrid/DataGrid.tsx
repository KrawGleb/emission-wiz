import { CellEditingStoppedEvent, CellKeyDownEvent, FullWidthCellKeyDownEvent, SizeColumnsToContentStrategy, SizeColumnsToFitGridStrategy, SizeColumnsToFitProvidedWidthStrategy } from 'ag-grid-community';
import { AgGridReact } from "ag-grid-react";
import { observer } from "mobx-react";
import React from "react";

import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-quartz.css";
import { action, computed, observable } from 'mobx';
import { DataGridColumn } from './DataGridColumn';

export type DataGridProps<T extends object> = {
    editable?: boolean;
    columns?: DataGridColumn<T>[];
    rowData?: T[];
    height: number | string;
    addEmptyRow?: boolean;
    suppressNoRowsOverlay?: boolean;
    autoSizeStrategy?: SizeColumnsToFitGridStrategy | SizeColumnsToFitProvidedWidthStrategy | SizeColumnsToContentStrategy;
    onChange?: () => void;
}

@observer
export default class DataGrid<T extends object> extends React.Component<DataGridProps<T>> {
    @observable
    private accessor _rows: T[] | undefined;

    @observable
    private accessor _gridRef = React.createRef<AgGridReact>();

    constructor(props: DataGridProps<T>) {
        super(props);
        this._rows = props.rowData ?? [];

        if (this.props.addEmptyRow) 
            this._rows.push({ } as T);
    }

    render() {
        const gridHeight = this.props.height;

        return (
            <div className="ag-theme-quartz mb-2" style={{ height: gridHeight }} >
                <AgGridReact
                    ref={this._gridRef}
                    suppressLoadingOverlay
                    suppressNoRowsOverlay={this.props.suppressNoRowsOverlay}
                    columnDefs={this.props.columns}
                    onCellEditingStopped={(event) => this._onCellEditingStoped(event)}
                    rowData={this._rows}
                    autoSizeStrategy={this.props.autoSizeStrategy ?? {
                        type: 'fitGridWidth',
                    }} 
                    onCellKeyDown={(event) => this._onCellKeyDown(event)}
                    rowSelection={'multiple'}
                    />
            </div>
        );
    }

    @computed
    public get rows() {
        const rowData: T[] = [];
        this._gridRef.current!.api.forEachNode(node => rowData.push(node.data))

        return rowData;
    }

    @action
    private _onCellEditingStoped(event: CellEditingStoppedEvent) {
        if (event.valueChanged && !event.oldValue)
            this._addEmptyRow();

        this.props.onChange?.();
    }

    @action
    private _addEmptyRow() {
        this._gridRef.current!.api.applyTransaction({
            add: [{}]
        })
    }

    @action
    private _onCellKeyDown(event: CellKeyDownEvent | FullWidthCellKeyDownEvent) {
        if (!this.props.editable) return;

        const keyboardEvent = event.event as KeyboardEvent;
        const key = keyboardEvent.key;

        if (key === 'Delete') {
            const selectedRows = this._gridRef.current!.api.getSelectedRows();
            this._gridRef.current!.api.applyTransaction({
                remove: selectedRows
            });

            if (this.rows.length == 0 && this.props.addEmptyRow) {
                this._addEmptyRow();
            }
        }
    }
}