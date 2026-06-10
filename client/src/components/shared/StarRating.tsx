import { clamp } from "../../lib/math";
import "./starRating.css";

interface StarRatingProps {
  /** Score on a 0–10 scale; each unit renders as half a star (so 10 = 5 full stars). */
  score: number;
  max?: number;
  color?: string;
}

const TOTAL_STARS = 5;

const StarRating = ({ score, max = 10, color }: StarRatingProps) => {
  const clamped = clamp(score, 0, max);
  const unitsPerStar = max / TOTAL_STARS;

  return (
    <span className="star-rating">
      {Array.from({ length: TOTAL_STARS }).map((_, i) => {
        const filledUnits = clamp(clamped - i * unitsPerStar, 0, unitsPerStar);
        const pct = (filledUnits / unitsPerStar) * 100;
        return (
          <span key={i} className="star-rating__star">
            <span className="star-rating__fill" style={{ width: `${pct}%`, color }} />
          </span>
        );
      })}
    </span>
  );
};

export default StarRating;
