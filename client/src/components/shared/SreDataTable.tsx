import DataTable, { TableProps } from "react-data-table-component";

interface SreTableProps<T> extends TableProps<T> {
    maxwidth?: number
}

function SreDataTable<T>(props: SreTableProps<T>) {
    return (
        <div style={{ width: props.maxwidth }} >
            <DataTable {...props} striped dense highlightOnHover theme="dark" />
        </div>
    )
}

export default SreDataTable;
