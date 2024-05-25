import { useEffect } from "react";
import "./switch.css";

const Switch = ({
  value,
  handleOnChange,
  sliderContent,
}: {
  value: boolean;
  handleOnChange: (newVal?: boolean) => void;
  sliderContent?: string;
}) => {
  useEffect(() => {});
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
