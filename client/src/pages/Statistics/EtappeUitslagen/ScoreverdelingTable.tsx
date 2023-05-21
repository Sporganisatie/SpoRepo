import DataTable, { TableColumn } from 'react-data-table-component';
import { useBudgetContext } from '../../../components/shared/BudgetContextProvider';

interface ScoreVerdeling {
    username: string;
    bin0: number;
    bin1: number;
    bin2: number;
    bin3: number;
    bin4: number;
}

const ScoreverdelingTable = ({ data }: any) => {
    const budgetParticipation = useBudgetContext();

    const bins = budgetParticipation ? ['10-', '10', '30', '50', '100+'] : ['50-', '50', '100', '200', '300+']

    const columns: TableColumn<ScoreVerdeling>[] = [
        {
            name: '',
            selector: (row: ScoreVerdeling) => row.username
        },
        {
            name: bins[0],
            selector: (row: ScoreVerdeling) => row.bin0
        },
        {
            name: bins[1],
            selector: (row: ScoreVerdeling) => row.bin1
        },
        {
            name: bins[2],
            selector: (row: ScoreVerdeling) => row.bin2
        },
        {
            name: bins[3],
            selector: (row: ScoreVerdeling) => row.bin3
        },
        {
            name: bins[4],
            selector: (row: ScoreVerdeling) => row.bin4
        }
    ];

    return (
        <DataTable
            title="Score Verdeling"
            columns={columns}
            data={data}
            highlightOnHover
            striped
            dense
        />
    );
};

export default ScoreverdelingTable;
