import React, { useEffect, useState } from "react";

const TWENTY_FOUR_HOURS_MS = 24 * 60 * 60 * 1000;

interface CountdownClock24HProps {
  targetDate: Date;
  className?: string;
}

function isWithin24h(targetDate: Date): boolean {
  return targetDate.getTime() - Date.now() < TWENTY_FOUR_HOURS_MS;
}

function formatTimeLeft(targetDate: Date): string {
  const difference = targetDate.getTime() - Date.now();
  if (difference <= 0) return "00:00:00";
  const hours = String(Math.floor((difference / (1000 * 60 * 60)) % 24)).padStart(2, "0");
  const minutes = String(Math.floor((difference / (1000 * 60)) % 60)).padStart(2, "0");
  const seconds = String(Math.floor((difference / 1000) % 60)).padStart(2, "0");
  return `${hours}:${minutes}:${seconds}`;
}

const CountdownClock24H: React.FC<CountdownClock24HProps> = ({ targetDate, className }) => {
  const visible = isWithin24h(targetDate);
  const [timeLeft, setTimeLeft] = useState(() => formatTimeLeft(targetDate));

  useEffect(() => {
    if (!visible) return;
    setTimeLeft(formatTimeLeft(targetDate));
    const interval = setInterval(() => {
      setTimeLeft((prev) => {
        const next = formatTimeLeft(targetDate);
        return next === prev ? prev : next;
      });
    }, 1000);
    return () => clearInterval(interval);
  }, [targetDate, visible]);

  if (!visible) return null;

  return <div className={`countdown-24h ${className ?? ""}`}>{timeLeft}</div>;
};

export { isWithin24h };
export default CountdownClock24H;
