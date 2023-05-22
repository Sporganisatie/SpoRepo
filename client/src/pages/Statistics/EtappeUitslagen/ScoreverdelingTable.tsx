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

const ScoreverdelingTable = ({ data, allRaces }: { data: any, allRaces: boolean }) => {
    const budgetParticipation = useBudgetContext();

    const stageBins = budgetParticipation ? ['10-', '10', '30', '50', '100+'] : ['50-', '50', '100', '200', '300+']
    const raceBins = budgetParticipation ? ['500-', '500', '750', '1000'] : ['4000-', '4000', '4500', '5000+']
    const bins = allRaces ? raceBins : stageBins;

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
            selector: (row: ScoreVerdeling) => row.bin4,
            omit: allRaces
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
