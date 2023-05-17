import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useBudgetContext } from "../../components/shared/BudgetContextProvider";
import axios from "axios";
import DataTable, { TableColumn } from "react-data-table-component";

export type UitvallersData = {
    userName: string;
    uitvallers: number;
    uitvallerBudget: number;
};

const columns: TableColumn<UitvallersData>[] = [
    {
        name: 'User',
        width: '80px',
        selector: (row: UitvallersData) => row.userName,
        sortable: true
    },
    {
        name: 'Aantal',
        width: '90px',
        selector: (row: UitvallersData) => row.uitvallers,
        sortable: true
    },
    {
        name: 'Budget',
        width: '90px',
        selector: (row: UitvallersData) => row.uitvallerBudget,
        sortable: true
    }
];

const Uitvallers = () => {
    document.title = "Uitvallers";
    let { raceId } = useParams();
    const budgetParticipation = useBudgetContext();
    const [data, setData] = useState<UitvallersData[]>([]);

    useEffect(() => {
        axios
            .get(`/api/Statistics/uitvallers`, { params: { raceId, budgetParticipation } })
            .then(res => {
                setData(res.data);
            })
            .catch(error => {
            });
    }, [raceId, budgetParticipation]);

    return (
        <div style={{ border: 'solid', width: '300px' }} >
            <DataTable
                title={"Uitvallers"}
                columns={columns}
                data={data}
                striped
                dense
            />
        </div>
    )
}

export default Uitvallers;