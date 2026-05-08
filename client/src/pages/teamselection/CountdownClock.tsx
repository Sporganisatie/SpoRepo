import React, { useEffect, useState } from "react";

interface CountdownClock24HProps {
  targetDate: Date;
  className?: string;
}

function formatTimeLeft(targetDate: Date): string {
  const difference = targetDate.getTime() - new Date().getTime();
  if (difference <= 0) return "00:00:00";
  const hours = String(Math.floor((difference / (1000 * 60 * 60)) % 24)).padStart(2, "0");
  const minutes = String(Math.floor((difference / (1000 * 60)) % 60)).padStart(2, "0");
  const seconds = String(Math.floor((difference / 1000) % 60)).padStart(2, "0");
  return `${hours}:${minutes}:${seconds}`;
}

const CountdownClock24H: React.FC<CountdownClock24HProps> = ({ targetDate, className }) => {
  const [timeLeft, setTimeLeft] = useState(() => formatTimeLeft(targetDate));

  useEffect(() => {
    setTimeLeft(formatTimeLeft(targetDate));
    const interval = setInterval(() => {
      setTimeLeft(formatTimeLeft(targetDate));
    }, 1000);
    return () => clearInterval(interval);
  }, [targetDate]);

  if (targetDate.getTime() - new Date().getTime() >= 24 * 60 * 60 * 1000) {
    return null;
  }

  return <div className={`countdown-24h ${className ?? ""}`}>{timeLeft}</div>;
};

export default CountdownClock24H;
