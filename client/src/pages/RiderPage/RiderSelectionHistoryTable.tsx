import Table from "@/components/ui/table/Table";
import type { RiderSelectionHistoryRow } from "./models/RiderOverview";

interface RiderSelectionHistoryTableProps {
    history: RiderSelectionHistoryRow[];
}

const RiderSelectionHistoryTable = ({ history }: RiderSelectionHistoryTableProps) => {
    if (history.length === 0) {
        return null;
    }

    return (
        <div className="panel rider-overview-table">
            <Table data={history} rowKey="username" pointerOnHover fitContent paginated>
                {(col) => [
                    col.text((r: RiderSelectionHistoryRow) => r.username, { name: "Naam", width: "120px" }),
                    col.text((r: RiderSelectionHistoryRow) => `${r.selectedCount} / ${r.totalCount}`, {
                        name: "Geselecteerd",
                        width: "90px",
                    }),
                    col.text(
                        (r: RiderSelectionHistoryRow) => {
                            const text = r.races.join(", ");
                            return (
                                <span
                                    title={text}
                                    style={{
                                        display: "block",
                                        overflow: "hidden",
                                        textOverflow: "ellipsis",
                                        whiteSpace: "nowrap",
                                        maxWidth: "500px",
                                    }}
                                >
                                    {text}
                                </span>
                            );
                        },
                        { name: "Races" }
                    ),
                ]}
            </Table>
        </div>
    );
};

export default RiderSelectionHistoryTable;
