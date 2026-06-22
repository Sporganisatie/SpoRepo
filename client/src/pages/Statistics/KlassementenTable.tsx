import type { InputData } from "./Klassementen";
import Table from "@/components/ui/table/Table";

const KlassementenTable = ({
  title,
  riders,
  resultTitle,
}: {
  title: string;
  resultTitle: string;
  riders: InputData[];
}) => (
  <Table data={riders ?? []} title={title} rowKey="position">
    {(col) => [
      col.text((r) => r.position, { name: "Positie", width: "70px" }),
      col.rider((r) => r.rider, { name: "Naam" }),
      col.text((r) => r.result, { name: resultTitle, width: "120px" }),
      col.text((r) => r.accounts.length, { name: "Gekozen", width: "100px" }),
      col.text((r) => r.accounts.join(", "), { name: "Users", width: "300px" }),
    ]}
  </Table>
);

export default KlassementenTable;
