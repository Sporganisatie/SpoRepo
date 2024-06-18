const SmallSwitch = ({ text, selected, index, toggleUser }: { text: string, selected: boolean, index: number, toggleUser: (index: number) => void }) =>
    <div
        style={{ display: 'inline-block', cursor: 'pointer', marginRight: '2px' }}
        onClick={() => toggleUser(index)}>
        <input type="checkbox" checked={selected} onChange={() => { }} style={{ cursor: 'pointer' }} />
        <span style={{ userSelect: 'none' }}>{text}</span>
    </div>

export default SmallSwitch;