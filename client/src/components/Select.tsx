import React, { useState, useEffect } from 'react';

export interface SelectOption<T extends string | number> {
    displayValue: string;
    value: T;
}

export interface SelectProps<T extends string | number> {
    options: SelectOption<T>[];
    onChange: (selectedOption: T) => void;
    value?: T;
}

function Select<T extends string | number>(props: SelectProps<T>) {
    const { options, onChange, value: initialValue } = props;

    const [value, setValue] = useState(initialValue);

    useEffect(() => {
        setValue(initialValue);
    }, [initialValue]);

    const handleChange = (event: React.ChangeEvent<HTMLSelectElement>) => {
        const selectedValue = event.target.value as unknown as T;
        onChange(selectedValue);
        setValue(selectedValue);
    };

    return (
        <div style={{ display: 'flex', alignItems: 'center' }}>
            <select onChange={handleChange} value={value}>
                {options.map((option, index) => (
                    <option key={index} value={option.value}>
                        {option.displayValue}
                    </option>
                ))}
            </select>
        </div>
    );
}

export default Select;
