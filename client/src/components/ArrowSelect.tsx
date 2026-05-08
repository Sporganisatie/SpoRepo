import { useEffect, useState } from "react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faChevronLeft, faChevronRight } from "@fortawesome/free-solid-svg-icons";
import type { SelectProps } from "./Select";
import Select from "./Select";

export interface ArrowSelectProps<T extends string | number> extends SelectProps<T> {
  allowLooping: boolean;
}

function ArrowSelect<T extends string | number>(props: ArrowSelectProps<T>) {
  const { options, onChange, value: initialValue, allowLooping } = props;
  const [currentIndex, setCurrentIndex] = useState<number>(
    options.findIndex((option) => option.value === initialValue)
  );

  useEffect(() => {
    const newIndex = options.findIndex((option) => option.value === initialValue);
    if (newIndex !== currentIndex) {
      setCurrentIndex(newIndex);
    }
  }, [initialValue, options, currentIndex]);

  const handleMove = (step: number) => {
    const newIndex = updateIndex(step);
    const newValue = options[newIndex].value;
    onChange(newValue);
    setCurrentIndex(newIndex);
  };

  const updateIndex = (step: number): number => {
    if (allowLooping) return (currentIndex + step + options.length) % options.length;
    const newIndex = currentIndex + step;
    if (newIndex < 0) return 0;
    if (newIndex >= options.length) return options.length - 1;
    return newIndex;
  };

  return (
    <div className="arrow-select">
      <button className="arrow-select__btn" onClick={() => handleMove(-1)} aria-label="Vorige">
        <FontAwesomeIcon icon={faChevronLeft} />
      </button>
      <Select
        options={options}
        onChange={(value) => {
          onChange(value);
          setCurrentIndex(options.findIndex((option) => option.value === value));
        }}
        value={options[currentIndex]?.value}
      />
      <button className="arrow-select__btn" onClick={() => handleMove(1)} aria-label="Volgende">
        <FontAwesomeIcon icon={faChevronRight} />
      </button>
    </div>
  );
}

export default ArrowSelect;
