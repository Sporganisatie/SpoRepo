import { useState } from 'react';
import Select, { SelectProps } from './Select';

export interface ArrowSelectProps<T extends string | number> extends SelectProps<T> {
    allowLooping: boolean;
}

function ArrowSelect<T extends string | number>(props: ArrowSelectProps<T>) {
    const { options, onChange, value: initialValue, allowLooping } = props;
    const [currentIndex, setCurrentIndex] = useState<number>(
        options.findIndex(option => option.value === initialValue)
    );

    const handleMove = (step: number) => {
        const newIndex = updateIndex(step);
        const newValue = options[newIndex].value;
        onChange(newValue);
        setCurrentIndex(newIndex);
    };

    const updateIndex = (step: number): number => {
        if (allowLooping) return (currentIndex + step + options.length) % options.length
        var newIndex = currentIndex + step;
        if (newIndex < 0) return 0;
        if (newIndex >= options.length) return options.length - 1;
        return newIndex;
    }

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
