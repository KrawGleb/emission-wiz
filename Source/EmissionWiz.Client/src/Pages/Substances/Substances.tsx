import React, { ReactNode } from "react";
import { Report } from "../../Components/Report";
import DataGrid from "../../Components/DataGrid/DataGrid";
import { SubstanceDto } from "../../Models/WebApiModels";
import { MathComponent } from "mathjax-react";
import { action, observable } from "mobx";
import { observer } from "mobx-react";
import ApiService from "../../Services/ApiService";
import { ApiUrls } from "../../AppConstants/ApiUrls";

@observer
export default class Substances extends React.Component {
  private _gridRef = React.createRef<DataGrid<SubstanceDto>>();

  @observable private accessor _substances: SubstanceDto[] = [];

  constructor(props: object) {
    super(props);

    void this._loadSubstances();
  }

  render(): ReactNode {
    return (
      <Report title="Вещества">
        <DataGrid<SubstanceDto>
          height="100%"
          ref={this._gridRef}
          rowData={this._substances}
          columns={[
            {
              field: "code",
              headerName: "Код",
            },
            {
              field: "name",
              headerName: "Наименование",
            },
            {
              field: "chemicalFormula",
              headerName: "Хим. формула",
            },
            {
              field: "dangerClass",
              headerName: "Класс опасности",
            },
            {
              field: "singleMaximumAllowableConcentration",
              headerComponent: () => <MathComponent tex="c" />,
              headerTooltip: "Максимально допустимая разовая концентрация",
            },
            {
              field: "dailyAverageMaximumAllowableConcentration",
              headerComponent: () => <MathComponent tex="c" />,
              headerTooltip:
                "Максимально допустимая среднедневная концентрация",
            },
            {
              field: "annualAverageMaximumAllowableConcentration",
              headerComponent: () => <MathComponent tex="c" />,
              headerTooltip:
                "Максимально допустимая среднегодовая концентрация",
            },
          ]}
        />
      </Report>
    );
  }

  @action.bound
  private async _loadSubstances() {
    const { data } = await ApiService.getTypedData<SubstanceDto[]>(
      ApiUrls.Substance
    );

    this._substances = data;
  }
}
