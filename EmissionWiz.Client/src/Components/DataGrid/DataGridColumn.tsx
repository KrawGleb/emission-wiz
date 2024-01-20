import { ColDef } from 'ag-grid-community';

export type DataGridColumn<T extends {}> =  ColDef<T> & {
    newRowValue?: (rowIndex: number) => string; 
}