import React, { useEffect, useState } from "react";

interface CountdownClock24HProps {
  targetDate: Date;
  className?: string;
}

const CountdownClock24H: React.FC<CountdownClock24HProps> = ({ targetDate, className }) => {
  const [timeLeft, setTimeLeft] = useState("");

  useEffect(() => {
    const interval = setInterval(() => {
      const now = new Date();
      const difference = targetDate.getTime() - now.getTime();

      if (difference <= 0) {
        clearInterval(interval);
        setTimeLeft("00:00:00");
        return;
      }

      const hours = String(Math.floor((difference / (1000 * 60 * 60)) % 24)).padStart(2, "0");
      const minutes = String(Math.floor((difference / (1000 * 60)) % 60)).padStart(2, "0");
      const seconds = String(Math.floor((difference / 1000) % 60)).padStart(2, "0");

      setTimeLeft(`${hours}:${minutes}:${seconds}`);
    }, 1000);

    return () => clearInterval(interval);
  }, [targetDate]);

  if (targetDate.getTime() - new Date().getTime() >= 24 * 60 * 60 * 1000) {
    return null;
  }

  return <div className={`countdown-24h ${className ?? ""}`}>{timeLeft}</div>;
};

export default CountdownClock24H;
