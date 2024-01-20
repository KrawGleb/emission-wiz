import { CellEditingStoppedEvent } from 'ag-grid-community';
import { AgGridReact } from "ag-grid-react";
import { observer } from "mobx-react";
import React from "react";

import "ag-grid-community/styles/ag-grid.css";
import "ag-grid-community/styles/ag-theme-quartz.css";
import { observable } from 'mobx';
import { DataGridColumn } from './DataGridColumn';

export type DataGridProps<T extends {}> = {
    columns?: DataGridColumn<T>[];
    rowData?: T[];
    height: number;
    addEmptyRow?: boolean;
}

@observer
export default class DataGrid<T extends {}> extends React.Component<DataGridProps<T>> {
    @observable
    private accessor _rowData: T[];

    @observable
    private accessor _gridRef = React.createRef<AgGridReact>();

    constructor(props: DataGridProps<T>) {
        super(props);
        this._rowData = props.rowData ?? [];

        if (this.props.addEmptyRow)
            this._addEmptyRow();
    }

    render() {
        const gridHeight = this.props.height;

        return (
            <div className="ag-theme-quartz mb-2" style={{ width: "100%", height: gridHeight }} >
                <AgGridReact
                    ref={this._gridRef}
                    suppressLoadingOverlay
                    columnDefs={this.props.columns}
                    onCellEditingStopped={(event) => this._onCellEditingStoped(event)}
                    rowData={this._rowData}
                    autoSizeStrategy={{
                        type: 'fitGridWidth',
                    }} />
            </div>
        );
    }

    private _onCellEditingStoped(event: CellEditingStoppedEvent) {
        if (event.valueChanged && event.rowIndex == this._rowData.length - 1 && !event.oldValue)
            this._addEmptyRow();
    }

    private _addEmptyRow() {
        this._rowData.push(this._getNewRow(this._rowData.length + 1));
        this._gridRef.current?.api.setGridOption('rowData', this._rowData);
    }

    private _getNewRow(rowIndex: number) {
        let obj = {} as T
        this.props.columns?.forEach((value) => {
            obj[value.field!.toString() as keyof T] = value.newRowValue ? value.newRowValue(rowIndex) as any : undefined;
        })

        return obj;
    }
}