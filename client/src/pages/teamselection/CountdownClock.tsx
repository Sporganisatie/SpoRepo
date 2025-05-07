import React, { useEffect, useState } from "react";

const CountdownClock24H: React.FC<{ targetDate: Date }> = ({ targetDate }) => {
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

    return (
        <>
            {targetDate.getTime() - new Date().getTime() < 24 * 60 * 60 * 1000 && (
                <div
                    style={{
                        fontSize: "3rem",
                        color: "red",
                        textAlign: "center",
                        margin: "1rem 0",
                        fontFamily: "monospace",
                    }}
                >
                    {timeLeft}
                </div>
            )}
        </>
    );
};

export default CountdownClock24H;