import { ExpanderComponentProps } from "react-data-table-component";
import { SelectableRider } from "./Models/SelectableRider";

const SelectableRiderFoldout: React.FC<ExpanderComponentProps<SelectableRider>> = ({ data }) => <div>
    {data.details.gc > 0 && <div>Klassement:{'★'.repeat(data.details.gc)}</div>}
    {data.details.sprint > 0 && <div>Sprint:{'★'.repeat(data.details.sprint)}</div>}
    {data.details.climb > 0 && <div>Klimmen:{'★'.repeat(data.details.climb)}</div>}
    {data.details.tt > 0 && <div>Tijdrijden:{'★'.repeat(data.details.tt)}</div>}
    {data.details.punch > 0 && <div>Punch:{'★'.repeat(data.details.punch)}</div>}
</div>;


export default SelectableRiderFoldout;