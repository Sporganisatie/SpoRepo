import { Rider } from '../../models/Rider';
import RiderCell from './Cells/RiderCell';
import './tempTable.css';
interface TableInput {
    headers: Header[]
    data: Row[]
    title?: string
}

interface Header {
    title: string;
    name: string;
    // https://mariusschulz.com/blog/tagged-union-types-in-typescript
    // in data of header gaat het aan union type Supported Table cell moeten voldoen
}

export interface Row {

}

const Table = (input: TableInput) => {

    return (
        <table>
            {/* TODO colgroup met widths, uitzoeken wat colgroup nog meer kan*/}
            {/* TODO caption/title */}
            <thead>
                <tr key="headers">
                    {input.headers.map(header => <th key={header.title}>{header.title}</th>)}
                </tr>
            </thead>
            <tbody>
                {BuildBody(input)}
            </tbody>
        </table>);
}

const BuildBody = (input: TableInput): JSX.Element[] => { // TODO merge met ConvertDataToRow
    return input.data.map((row, index) => ConvertDataToRow(row, input.headers, index))
}

const ConvertDataToRow = (row: Row, headers: Header[], index: number): JSX.Element => {
    var cells = [];
    for (var header of headers) {
        type ObjectKey = keyof typeof row;
        const myVar = header.name as ObjectKey;
        var data = row[myVar];
        // var type = typeof data === 'object' ? data.type : typeof data;
        cells.push(BuildCell(header.name, data, index))
    }
    return <tr key={index}>{cells}</tr>
}

const BuildCell = (name: string, data: Row, index: number) => { //TODO dit moet niet met een switch maar met automatische types
    switch (name) {
        case "stagePosition":
            return <td key={index + name}>{data.toString() + "e"}</td>
        case "rider":
            return <RiderCell key={index + name} rider={data as Rider} />
        default:
            return <td key={index + name}>{data.toString()}</td>
    }
}

export default Table;

