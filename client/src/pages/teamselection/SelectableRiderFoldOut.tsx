import { ExpanderComponentProps } from "react-data-table-component";
import { SelectableRider } from "./Models/SelectableRider";

const SelectableRiderFoldout: React.FC<ExpanderComponentProps<SelectableRider>> = ({ data }) => <div>
    {<div>Klassement: {data.details.gc > 0 ? '★'.repeat(data.details.gc) : "-"}</div>}
    {<div>Sprint: {data.details.sprint > 0 ? '★'.repeat(data.details.sprint) : "-"}</div>}
    {<div>Klimmen: {data.details.climb > 0 ? '★'.repeat(data.details.climb) : "-"}</div>}
    {<div>Tijdrijden: {data.details.tt > 0 ? '★'.repeat(data.details.tt) : "-"}</div>}
    {<div>Punch: {data.details.punch > 0 ? '★'.repeat(data.details.punch) : "-"}</div>}
    <br></br>
</div>;


export default SelectableRiderFoldout;