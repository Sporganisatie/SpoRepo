interface TableInput {
    headers: string[]
    data: object[]
}

const Table = (props: TableInput) => {
    var headers: any[] = [];
    props.headers.forEach(x => headers.push(<th>{x}</th>))
    return (
        <div>
            <table>
                {/* TODO colgroup met widths, uitzoeken wat colgroup nog meer kan*/}
                {/* TODO caption/title */}
                <thead>
                    <tr key="headers">
                        {headers}
                    </tr>
                </thead>
                <tbody>

                </tbody>
            </table>
        </div>);
}

export default Table;
