import { useEffect } from "react";
import "./switch.css";

const Switch = ({
    value,
    handleOnChange,
    sliderContent,
    hotkey,
}: {
    value: boolean;
    handleOnChange: (newVal?: boolean) => void;
    sliderContent?: string;
    hotkey?: string;
}) => {
    useEffect(() => {
        if (!hotkey) {
            return;
        }
        function handleKeydown(e: KeyboardEvent) {
            console.log();
            if (!hotkey || e.target instanceof HTMLInputElement) {
                return;
            }
            if (e.key.toLowerCase() === hotkey.toLowerCase()) {
                handleOnChange(!value);
            }
        }

        window.addEventListener("keydown", handleKeydown);

        return () => {
            window.removeEventListener("keydown", handleKeydown);
        };
    });
    return (
        <label className="switch">
            <input
                type="checkbox"
                checked={value}
                onChange={(val) => handleOnChange(val.target.checked)}
            />
            <span className="slider round"></span>
            <div className="slider-content">{sliderContent}</div>
        </label>
    );
};

export default Switch;
