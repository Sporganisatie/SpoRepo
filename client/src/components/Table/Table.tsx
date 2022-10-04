import RiderCell from './Cells/RiderCell';
import './tempTable.css';
interface TableInput {
    headers: Header[]
    data: object[] // TODO maak class voor tablerow type
    title?: string
}

interface Header {
    title: string;
    name: string;
    // https://mariusschulz.com/blog/tagged-union-types-in-typescript
    // in data of header gaat het aan union type Supported Table cell moeten voldoen
}

const Table = (props: TableInput) => {
    var headers: any[] = [];
    props.headers.forEach(x => headers.push(<th key={x.title}>{x.title}</th>))
    var bodyRows = props.data.map((row, index) => ConvertDataToRow(row, props.headers, index))
    return (
        <table>
            {/* TODO colgroup met widths, uitzoeken wat colgroup nog meer kan*/}
            {/* TODO caption/title */}
            <thead>
                <tr key="headers">
                    {headers}
                </tr>
            </thead>
            <tbody>
                {bodyRows}
            </tbody>
        </table>);
}

const ConvertDataToRow = (row: any, headers: Header[], index: number): any => { //TODO response/input types, rename to build body
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

const BuildCell = (name: string, data: any, index: number) => { //TODO response/input types
    switch (name) {
        case "stagePosition":
            return <td key={index + name}>{data.toString() + "e"}</td>
        case "rider":
            return <RiderCell key={index + name} rider={data} />
        default:
            return <td key={index + name}>{data.toString()}</td>
    }
}

export default Table;

