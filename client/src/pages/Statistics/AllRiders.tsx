import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useBudgetContext } from "../../components/shared/BudgetContextProvider";
import axios from "axios";
import DataTable, { TableColumn } from "react-data-table-component";
import { RiderParticipation } from "../../models/RiderParticipation";
import RiderLink from "../../components/shared/RiderLink";

export type AllRiderRow = {
    riderParticipation: RiderParticipation;
    stageScore: number;
    klassementen: number;
    teamScore: number;
    totalScore: number;
    totalSelected: number;
    accounts: string[];
};

const AllRiders = () => {
    document.title = "AllRiders";
    let { raceId } = useParams();
    const budgetParticipation = useBudgetContext();
    const [data, setData] = useState<AllRiderRow[]>([]);

    const columns: TableColumn<AllRiderRow>[] = [
        {
            name: 'Naam',
            width: '200px',
            cell: (row: AllRiderRow) => <RiderLink rider={row.riderParticipation.rider} />,
            sortable: true
        },
        {
            name: 'Totaal',
            width: '100px',
            selector: (row: AllRiderRow) => row.totalScore,
            sortable: true
        },
        {
            name: 'Prijs',
            width: '100px',
            selector: (row: AllRiderRow) => row.riderParticipation.price / 1000000,
            sortable: true
        },
        {
            name: 'Geselecteerd',
            width: '100px',
            selector: (row: AllRiderRow) => row.totalSelected,
            sortable: true
        },
        {
            name: 'Users',
            width: '300px',
            selector: (row: AllRiderRow) => row.accounts.sort((a, b) => (a > b ? 1 : -1)).join(", "),
            sortable: true
        },
        {
            name: 'Etappe',
            width: '100px',
            selector: (row: AllRiderRow) => row.stageScore,
            sortable: true
        },
        {
            name: 'Klassementen',
            width: '100px',
            selector: (row: AllRiderRow) => row.klassementen,
            sortable: true
        },
        {
            name: 'Team',
            width: '100px',
            selector: (row: AllRiderRow) => row.teamScore,
            sortable: true,
            omit: budgetParticipation
        },
        {
            name: 'P/M',
            width: '100px',
            selector: (row: AllRiderRow) => Math.round(row.totalScore / row.riderParticipation.price * 1000000),
            sortable: true
        }
    ];

    useEffect(() => {
        axios
            .get(`/api/Statistics/allRiders`, { params: { raceId, budgetParticipation } })
            .then(res => {
                setData(res.data);
            })
            .catch(error => {
            });
    }, [raceId, budgetParticipation]);

    return (
        <div>
            <DataTable
                title={"Alle Renners"}
                columns={columns}
                data={data}
                striped
                dense
            />
        </div>
    )
}

export default AllRiders;