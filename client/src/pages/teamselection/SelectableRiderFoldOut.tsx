import { ExpanderComponentProps } from "react-data-table-component";
import { SelectableRider } from "./Models/SelectableRider";

const SelectableRiderFoldout: React.FC<ExpanderComponentProps<SelectableRider>> = ({ data }) => <div>
    GC:{data.details.gc}
</div>;


export default SelectableRiderFoldout;