import React from 'react';
import { action, observable } from 'mobx';
import { observer } from 'mobx-react';
import { MathComponent } from 'mathjax-react';
import { ICellRendererParams } from 'ag-grid-community';
import { Button, Col, Collapse, CollapseProps, Divider, Row, Switch, Tooltip, TooltipProps } from 'antd';
import { BorderOutlined, CloseOutlined, DownloadOutlined, EllipsisOutlined, SettingOutlined } from '@ant-design/icons';

import { BaseFormModel } from '../../Models/BaseFromModel';
import { Report } from '../../Components/Report';
import ApiService from '../../Services/ApiService';
import { ApiUrls } from '../../AppConstants/ApiUrls';
import { FileContent, FileContentType, SingleSourceEmissionCalculationResult, SingleSourceEmissionSubstance, SingleSourceInputModel, SingleSourceResultsConfig, SubstanceDto } from '../../Models/WebApiModels';
import { FormInput } from '../../Components/FormControls';
import { collections, displayName, isNumber, isRequired } from '../../Services/Validation';
import { downloadService } from '../../Services/DownloadService';
import PdfViewer from '../../Components/PdfViewer';
import MapContainer, { UniqueMarker } from '../../Components/MapContainer';
import DataGrid from '../../Components/DataGrid/DataGrid';
import { DataGridColumn } from '../../Components/DataGrid/DataGridColumn';
import WindRose, { WindDirection } from '../../Components/WindRose';
import { ModalButtonType, modalService } from '../../Components/Modal/Modal';
import { SingleSourceResultsConfigurationDialog, SingleSourceResultsConfigurationDialogProps } from '../../Components/Dialogs/SingleSourceResultsConfigurationDialog';
import DataGridDropdownCell from '../../Components/DataGrid/DataGridDropdownCell';

class FormModel extends BaseFormModel {
    @isRequired()
    @isNumber()
    @observable
    public accessor a: number | undefined;

    @isRequired()
    @isNumber()
    @observable
    public accessor h: number | undefined;

    @isRequired()
    @isNumber()
    @observable
    public accessor f: number | undefined;

    @displayName('Тв')
    @isRequired()
    @isNumber()
    @observable
    public accessor airTemperature: number | undefined;

    @displayName('Тг')
    @isRequired()
    @isNumber()
    @observable
    public accessor emissionTemperature: number | undefined;

    @isRequired()
    @isNumber()
    @observable
    public accessor d: number | undefined;

    @isRequired()
    @isNumber()
    @observable
    public accessor eta: number | undefined;

    @isRequired()
    @isNumber()
    @observable
    public accessor w: number | undefined;

    @isRequired()
    @isNumber()
    @observable
    public accessor u: number | undefined;

    @isRequired()
    @isNumber()
    @observable
    public accessor x: number | undefined;

    @isRequired()
    @isNumber()
    @observable
    public accessor y: number | undefined;

    @isNumber()
    public accessor b: number | undefined;

    @isNumber()
    public accessor l: number | undefined;

    @observable
    public accessor isReactangle: boolean = false;

    @observable
    public accessor windSpeed: WindDirection[] = [];

    @observable
    @collections.notEmpty()
    @collections.numbers('_lngLat.lat')
    @collections.numbers('_lngLat.lng')
    public accessor markers: UniqueMarker[] = [];

    @observable
    @collections.notEmpty()
    @collections.numbers('m')
    public accessor substances: SingleSourceEmissionSubstance[] = [];
}

export class SingleSourceEmissionSubstanceCell {
    id: string;
    name: string;
    m: number;
    nameOption: {
        value: string;
        label: string;
    }
}

@observer
export default class SingleSource extends React.Component {
    @observable private accessor _calculationResults: SingleSourceEmissionCalculationResult[];
    @observable private accessor _reports: Map<string, Blob | null> = new Map<string, Blob | null>();
    @observable private accessor _resultsConfig: SingleSourceResultsConfig | undefined;
    @observable private accessor _substances: SubstanceDto[];

    private _form: FormModel = new FormModel();
    private _gridRef = React.createRef<DataGrid<SingleSourceEmissionSubstanceCell>>();
    private _mapRef = React.createRef<MapContainer<FormModel>>();

    constructor(props: object) {
        super(props);
        void this._loadSubstances();
    }

    render() {
        return (
            <Report title="Метод расчета максимальных разовых концентраций от выбросов одиночного точечного источника">
                <div className="d-flex flex-row">
                    <div style={{ width: '33%' }}>
                        <Divider orientation="left">
                            <h4>Вводные данные:</h4>
                        </Divider>
                        <div>
                            <MapContainer className="mb-2" formModel={this._form} name="markers" ref={this._mapRef} multiple />
                            {this._form.markers && (
                                <>
                                    {this._form.markers.map((marker, index) => (
                                        <div className="d-flex flex-row">
                                            <div className="d-flex flex-row" style={{ gap: '20px' }}>
                                                <FormInput
                                                    formModel={this._form}
                                                    name="markers"
                                                    index={index}
                                                    subName="_lngLat.lat"
                                                    placeholder="lat"
                                                    style={{ width: '90px' }}
                                                    value={marker._lngLat?.lat}
                                                    changeHandler={() => {
                                                        this._form.validate();
                                                        this._mapRef.current?.updateMarkers();
                                                    }}
                                                />

                                                <FormInput
                                                    formModel={this._form}
                                                    name="markers"
                                                    index={index}
                                                    subName="_lngLat.lng"
                                                    placeholder="lon"
                                                    style={{ width: '90px' }}
                                                    value={marker._lngLat?.lng}
                                                    changeHandler={() => {
                                                        this._form.validate();
                                                        this._mapRef.current?.updateMarkers();
                                                    }}
                                                />
                                            </div>
                                            <div style={{ marginLeft: '5px', cursor: 'pointer' }}>
                                                <CloseOutlined style={{ fontSize: '12px', verticalAlign: 'super' }} onClick={() => this._removeMarker(marker)} />
                                            </div>
                                        </div>
                                    ))}
                                </>
                            )}
                        </div>
                        <div className="mb-2">
                            <Tooltip title="Форма устья">
                                <Switch
                                    checkedChildren={<BorderOutlined />}
                                    unCheckedChildren={<EllipsisOutlined />}
                                    defaultChecked={false}
                                    onChange={(checked) => {
                                        this._form.isReactangle = checked;
                                        if (!checked) {
                                            this._form.b = undefined;
                                            this._form.l = undefined;
                                        }
                                    }}
                                />
                            </Tooltip>
                        </div>
                        <Row gutter={14}>
                            <Col span={5}>
                                <FormInput
                                    label="A"
                                    formModel={this._form}
                                    name="a"
                                    placeholder="A"
                                    style={{ width: '90px' }}
                                    value={this._form.a}
                                    tooltip={
                                        {
                                            trigger: 'focus',
                                            title: 'Коэффициент, зависящий от температурной стратификации атмосферы, определяющий условия горизонтального и вертикального рассеивания ЗВ в атмосферном воздухе',
                                            placement: 'topLeft'
                                        } as TooltipProps
                                    }
                                />
                            </Col>

                            <Col span={5}>
                                <FormInput
                                    label="F"
                                    formModel={this._form}
                                    name="f"
                                    placeholder="F"
                                    style={{ width: '90px' }}
                                    value={this._form.f}
                                    tooltip={
                                        {
                                            trigger: 'focus',
                                            title: 'Безразмерный коэффициент, учитывающий скорость оседания ЗВ (газообразных и аэрозолей, включая твердые частицы) в атмосферном воздухе',
                                            placement: 'topLeft'
                                        } as TooltipProps
                                    }
                                />
                            </Col>

                            <Col span={5}>
                                <FormInput
                                    label="η"
                                    formModel={this._form}
                                    name="eta"
                                    placeholder="η"
                                    style={{ width: '90px' }}
                                    value={this._form.eta}
                                    tooltip={
                                        {
                                            trigger: 'focus',
                                            title: 'Безразмерный коэффициент, учитывающий влияние рельефа местности',
                                            placement: 'topLeft'
                                        } as TooltipProps
                                    }
                                />
                            </Col>
                            <Col span={5}>
                                <FormInput
                                    label="H"
                                    formModel={this._form}
                                    name="h"
                                    placeholder="H"
                                    style={{ width: '90px' }}
                                    value={this._form.h}
                                    suffix="м"
                                    tooltip={
                                        {
                                            trigger: 'focus',
                                            title: 'Высота источника выброса, м',
                                            placement: 'topLeft'
                                        } as TooltipProps
                                    }
                                />
                            </Col>

                            <Col span={5}>
                                <FormInput
                                    label="D"
                                    formModel={this._form}
                                    name="d"
                                    placeholder="D"
                                    style={{ width: '90px' }}
                                    value={this._form.d}
                                    suffix="м"
                                    tooltip={
                                        {
                                            trigger: 'focus',
                                            title: 'Диаметр устья источника выброса, м',
                                            placement: 'topLeft'
                                        } as TooltipProps
                                    }
                                />
                            </Col>

                            <Col span={5}>
                                <FormInput
                                    label="Тг"
                                    formModel={this._form}
                                    name="emissionTemperature"
                                    placeholder="Tг"
                                    style={{ width: '90px' }}
                                    value={this._form.emissionTemperature}
                                    suffix="°C"
                                    tooltip={
                                        {
                                            trigger: 'focus',
                                            title: 'Температура выбрасываемой ГВС, °C',
                                            placement: 'topLeft'
                                        } as TooltipProps
                                    }
                                />
                            </Col>

                            <Col span={5}>
                                <FormInput
                                    label="Тв"
                                    formModel={this._form}
                                    name="airTemperature"
                                    placeholder="Tв"
                                    style={{ width: '90px' }}
                                    value={this._form.airTemperature}
                                    suffix="°C"
                                    tooltip={
                                        {
                                            trigger: 'focus',
                                            title: 'Температурой атмосферного воздуха, °C',
                                            placement: 'topLeft'
                                        } as TooltipProps
                                    }
                                />
                            </Col>

                            <Col span={5}>
                                <FormInput
                                    label="W"
                                    formModel={this._form}
                                    name="w"
                                    placeholder="W"
                                    style={{ width: '90px' }}
                                    value={this._form.w}
                                    suffix="м/c"
                                    tooltip={
                                        {
                                            trigger: 'focus',
                                            title: 'Средняя скорость выхода ГВС из устья источника выброса, м/с',
                                            placement: 'topLeft'
                                        } as TooltipProps
                                    }
                                />
                            </Col>

                            <Col span={5}>
                                <FormInput
                                    label="U"
                                    formModel={this._form}
                                    name="u"
                                    placeholder="U"
                                    style={{ width: '90px' }}
                                    value={this._form.u}
                                    suffix="м/c"
                                    tooltip={
                                        {
                                            trigger: 'focus',
                                            title: 'Скорость ветра, м/с',
                                            placement: 'topLeft'
                                        } as TooltipProps
                                    }
                                />
                            </Col>

                            {this._form.isReactangle && (
                                <>
                                    <Col span={5}>
                                        <FormInput
                                            formModel={this._form}
                                            name="b"
                                            placeholder="B"
                                            style={{ width: '90px' }}
                                            value={this._form.b}
                                            suffix="м"
                                            tooltip={
                                                {
                                                    trigger: 'focus',
                                                    title: 'Ширина устья, м',
                                                    placement: 'topLeft'
                                                } as TooltipProps
                                            }
                                        />
                                    </Col>

                                    <Col span={5}>
                                        <FormInput
                                            formModel={this._form}
                                            name="l"
                                            placeholder="L"
                                            style={{ width: '90px' }}
                                            value={this._form.l}
                                            suffix="м"
                                            tooltip={
                                                {
                                                    trigger: 'focus',
                                                    title: 'Длина устья, м',
                                                    placement: 'topLeft'
                                                } as TooltipProps
                                            }
                                        />
                                    </Col>
                                </>
                            )}
                        </Row>
                        <div>
                            <WindRose formModel={this._form} name="windSpeed" />
                        </div>
                        <div>
                            <DataGrid<SingleSourceEmissionSubstanceCell>
                                editable
                                ref={this._gridRef}
                                rowData={this._form.substances.map(x => ({
                                    ...x,
                                    nameOption: {
                                        label: x.name,
                                        value: x.id
                                    }
                                }))}
                                columns={[
                                    {
                                        field: 'nameOption',
                                        editable: true,
                                        headerName: 'Наименование',
                                        cellEditor: DataGridDropdownCell,
                                        cellEditorParams: {
                                            values: this._substances?.map((x) => ({
                                                label: x.name,
                                                value: x.code
                                            })) ?? []
                                        },
                                        cellRenderer: (e: ICellRendererParams<SingleSourceEmissionSubstanceCell>) => e.getValue?.()?.label ?? '',
                                        onCellValueChanged: (event) => {
                                            event.data.name = event.data.nameOption?.label;
                                            event.data.id = event.data.nameOption?.value;

                                            if (!event.data.m) {
                                                event.data.m = 0;
                                            }

                                            event.api.applyTransaction({
                                                update: [event.data]
                                            });
                                        }
                                    },
                                    {
                                        field: 'm',
                                        editable: true,
                                        headerName: 'M (г/c)',
                                        headerTooltip: 'Масса ЗВ, выбрасываемого в атмосферный воздух в единицу времени (мощность выброса), г/с',
                                        onCellValueChanged: (event) => {
                                            if (!event.data.name) {
                                                event.data.name = `Вещество ${(event.node?.rowIndex ?? 0) + 1}`;
                                                event.api.applyTransaction({
                                                    update: [event.data]
                                                });
                                            }
                                        }
                                    }
                                ]}
                                height={200}
                                addEmptyRow
                                onChange={() => this._collectSubstances()}
                            />
                        </div>

                        <div className="d-flex flex-row mb-2" style={{ gap: '20px' }}>
                            <Button type="primary" style={{ width: '80px', padding: '0px' }} onClick={() => this._calculate()} disabled={!this._form.isFormValid}>
                                Рассчитать
                            </Button>
                            <Button type="primary" style={{ width: '280px' }} onClick={() => this._fillWithTestData()}>
                                Заполнить тестовыми данными
                            </Button>
                        </div>
                    </div>
                    <div style={{ width: '66%' }} className="d-flex flex-column">
                        {!!this._calculationResults && (
                            <Divider orientation="left" style={{ width: '80%' }}>
                                <h4>Результаты:</h4>
                            </Divider>
                        )}
                        <Row style={{ gap: '4px', flexDirection: 'row-reverse' }}>
                            <Button icon={<SettingOutlined />} type="link" onClick={() => this._openResultsConfigurationDialog()}>
                                Настройка результатов
                            </Button>
                        </Row>
                        {!!this._calculationResults && (
                            <>
                                <div className="p-2">
                                    {this._renderSolutions()}
                                    {this._renderResults()}
                                </div>
                            </>
                        )}
                    </div>
                </div>
            </Report>
        );
    }

    private _renderSolutions() {
        const items: CollapseProps['items'] = this._calculationResults.map((result, index) => {
            return {
                key: `solution_base${index}`,
                label: `Вычисления (${result.name})`,
                children: this._renderSolution(result, index)
            };
        });

        return <Collapse items={items} />;
    }

    private _renderSolution(result: SingleSourceEmissionCalculationResult, index: number) {
        const items: CollapseProps['items'] = [];

        result.files
            .slice()
            .sort((a, b) => a.sortOrder - b.sortOrder)
            .forEach((value, i) => {
                items.push({
                    key: value.type + `_${index}_${i}`,
                    label: value.name,
                    children: this._renderFileContentPreview(value),
                    extra: (
                        <DownloadOutlined
                            onClick={(event) => {
                                event.stopPropagation();
                                this._downloadFile(value.fileId);
                            }}
                        />
                    )
                });
            });

        return <Collapse items={items} />;
    }

    private _renderFileContentPreview(fileContent: FileContent) {
        if (fileContent.type === FileContentType.Image) {
            return (
                <Row style={{ justifyContent: 'center' }}>
                    <img src={`/api/tiff?id=${fileContent.fileId}`} style={{ maxWidth: '100%' }} />
                </Row>
            );
        } else if (fileContent.type === FileContentType.Pdf) {
            const pdf = this._reports.get(fileContent.fileId);
            return <PdfViewer pdfData={pdf!} />;
        } else {
            return <div>Предпросмотр недоступен.</div>;
        }
    }

    private _renderResults() {
        const numberCellRenderer = (value: ICellRendererParams<SingleSourceEmissionCalculationResult>) => {
            return (value.getValue?.() as number).toFixed(4);
        };

        const columns: DataGridColumn<SingleSourceEmissionCalculationResult>[] = [
            {
                field: 'name',
                headerName: '',
                pinned: 'left'
            },
            {
                field: 'cm',
                headerComponent: () => <MathComponent tex="c_{m}, \frac{mg}{m^3}" />,
                headerTooltip: 'Максимальная приземная разовая концентрация ЗВ',
                cellRenderer: numberCellRenderer
            },
            {
                field: 'cmu',
                headerComponent: () => <MathComponent tex="c_{mu}, \frac{mg}{m^3}" />,
                headerTooltip: 'Максимальная приземная концентрация ЗВ',
                cellRenderer: numberCellRenderer
            },
            {
                field: 'um',
                headerComponent: () => <MathComponent tex="u_{m}, \frac{m}{s}" />,
                headerTooltip: 'Опасная скорость ветра',
                cellRenderer: numberCellRenderer
            },
            {
                field: 'xm',
                headerComponent: () => <MathComponent tex="x_{m}, m" />,
                cellRenderer: numberCellRenderer
            },
            {
                field: 'xmu',
                headerComponent: () => <MathComponent tex="x_{mu}, m" />,
                headerTooltip: 'Расстояние от источника выброса, на котором при скорости ветра при неблагоприятных метеорологических условиях достигается максимальная приземная концентрация ЗВ',
                cellRenderer: numberCellRenderer
            }
        ];

        return (
            <div className="mt-2">
                <DataGrid<SingleSourceEmissionCalculationResult>
                    suppressNoRowsOverlay
                    autoSizeStrategy={{
                        type: 'fitCellContents'
                    }}
                    columns={columns}
                    rowData={this._calculationResults}
                    height={(this._calculationResults.length * 100) % 500}
                />
            </div>
        );
    }

    private async _openResultsConfigurationDialog() {
        const resultsConfig = await modalService.show<SingleSourceResultsConfigurationDialogProps, SingleSourceResultsConfig>(SingleSourceResultsConfigurationDialog, {
            printMap: this._resultsConfig?.printMap,
            highlightValue: this._resultsConfig?.highlightValue,
            acceptableError: this._resultsConfig?.acceptableError,
            includeGeoTiffData: this._resultsConfig?.includeGeoTiffData
        });
        if (resultsConfig.button === ModalButtonType.Save) this._resultsConfig = resultsConfig.result ?? undefined;
    }

    @action
    private _fillWithTestData() {
        this._form.a = 200;
        this._form.h = 30;
        this._form.d = 1;
        this._form.airTemperature = 24.6;
        this._form.emissionTemperature = 90;
        this._form.eta = 1;
        this._form.f = 1;
        this._form.x = 200;
        this._form.u = 20;
        this._form.w = 1;
        this._form.y = 2;
    }

    @action.bound
    private async _calculate() {
        const model = this._getModel();
        const { data } = await ApiService.postTypedData<SingleSourceEmissionCalculationResult[]>(ApiUrls.SingleSource, model);
        this._calculationResults = data;

        this._calculationResults.forEach(async (value) => {
            for (const file of value.files) {
                if (file.type === FileContentType.Pdf) {
                    const data = await this._loadPdf(file.fileId);
                    this._reports.set(file.fileId, data);
                }
            }
        });
    }

    @action.bound
    private async _downloadFile(id: string) {
        downloadService.downloadFile(`${ApiUrls.TempFile}?id=${id}`);
    }

    @action
    private _removeMarker(marker: UniqueMarker) {
        this._form.markers = this._form.markers.filter((x) => x !== marker);
        this._mapRef.current?.updateMarkers();
    }

    @action.bound
    private async _loadSubstances() {
        const { data } = await ApiService.getTypedData<SubstanceDto[]>(ApiUrls.Substance);

        this._substances = data;
    }

    private async _loadPdf(reportId: string) {
        const { data } = await ApiService.getTypedData<Blob>(`${ApiUrls.TempFile}?id=${reportId}`, null, {
            responseType: 'blob'
        });

        return data;
    }

    private _collectSubstances() {
        const rows = this._gridRef?.current?.rows;
        this._form.substances =
            rows
                ?.filter((v) => v.m !== undefined)
                .map((v) => ({
                    id: v.id,
                    name: v.name,
                    m: v.m
                })) ?? [];
    }

    private _getModel() {
        return {
            a: this._form.a!,
            fCoef: this._form.f!,
            h: this._form.h!,
            d: this._form.d!,
            w: this._form.w!,
            eta: this._form.eta!,
            airTemperature: this._form.airTemperature!,
            emissionTemperature: this._form.emissionTemperature!,
            u: this._form.u!,
            x: this._form.x!,
            y: this._form.y!,
            lat: this._form.markers[0]._lngLat.lat,
            lon: this._form.markers[0]._lngLat.lng,
            b: this._form.b,
            l: this._form.l,
            substances: this._form.substances,
            windRose: this._form.windSpeed,
            resultsConfig: this._resultsConfig
        } as SingleSourceInputModel;
    }
}
