import { TableColumn } from 'react-data-table-component';
import SreDataTable from '../../../components/shared/SreDataTable';

interface UserRank {
    username: string;
    ranks: number[]
}

const RankingTable = ({ data }: any) => {
    const highestNonZeroIndex = data.map((user: UserRank) => {
        const nonZeroIndices = user.ranks.map((rank, index) => {
            if (rank !== 0) {
                return index;
            }
            return 0;
        });
        return Math.max(...nonZeroIndices);
    });

    const length = Math.max(...highestNonZeroIndex) + 1;
    const additionalColumns = Array.from({ length }, (_, i) => i);
    const columns: TableColumn<UserRank>[] = [
        {
            name: 'User',
            selector: (row: UserRank) => row.username,
            width: '80px'
        },
        ...additionalColumns.map((column) => (
            {
                name: `${column + 1}e`,
                selector: (row: UserRank) => row.ranks[column],
                width: '50px'
            }))
    ];

    return <SreDataTable title="Aantal keer per positie" columns={columns} data={data} />
};

export default RankingTable;
