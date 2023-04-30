import { useState } from 'react';
import Select, { SelectProps } from './Select';

function ArrowSelect<T extends string | number>(props: SelectProps<T>) {
    const { options, onChange, value: initialValue } = props;
    const [currentIndex, setCurrentIndex] = useState<number>(
        options.findIndex(option => option.value === initialValue)
    );

    const handleMove = (step: number) => {
        const newIndex = (currentIndex + step + options.length) % options.length;
        const newValue = options[newIndex].value;
        onChange(newValue);
        setCurrentIndex(newIndex);
    };

    return (
        <div style={{ display: 'flex', alignItems: 'center' }}>
            <button onClick={() => handleMove(-1)}>{'<'}</button>
            <Select
                options={options}
                onChange={value => {
                    onChange(value);
                    setCurrentIndex(options.findIndex(option => option.value === value));
                }}
                value={options[currentIndex]?.value}
            />
            <button onClick={() => handleMove(1)}>{'>'}</button>
        </div>
    );
}

export default ArrowSelect;
