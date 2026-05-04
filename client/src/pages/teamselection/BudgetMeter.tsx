interface BudgetMeterProps {
  used: number;
  total: number;
}

function formatM(value: number): string {
  return `${(value / 1_000_000).toFixed(2)}M`;
}

const BudgetMeter = ({ used, total }: BudgetMeterProps) => {
  const remaining = total - used;
  const pct = total > 0 ? Math.min(100, Math.max(0, (used / total) * 100)) : 0;
  const over = remaining < 0;
  const warning = !over && remaining < total * 0.1;

  const fillClass = over ? "over" : warning ? "warning" : "";
  const textClass = over ? "over" : "";

  return (
    <div className="budget-meter" aria-label="Budget">
      <div className="budget-meter-track">
        <div
          className={`budget-meter-fill ${fillClass}`}
          style={{ width: `${over ? 100 : pct}%` }}
        />
      </div>
      <div className={`budget-meter-text ${textClass}`}>
        {formatM(used)} / {formatM(total)}
      </div>
    </div>
  );
};

export default BudgetMeter;
