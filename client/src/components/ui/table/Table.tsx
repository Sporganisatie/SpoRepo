import { useState, type ReactNode } from "react";
import type { Rider } from "../../../models/Rider";
import RiderLink from "../../shared/RiderLink";
import "./table.css";

type Align = "left" | "right" | "center";

interface TableColumn<T> {
  name: ReactNode;
  width?: string;
  cell: (row: T, index: number) => ReactNode;
  align?: Align;
  padding?: string;
  sortable?: boolean;
  sortFn?: (a: T, b: T) => number;
}

interface PositionOpts {
  name?: string;
  width?: string;
  ordinal?: boolean;
  align?: Align;
  padding?: string;
}

interface RiderOpts<T> {
  name?: string;
  width?: string;
  kopman?: (row: T) => boolean;
  fallback?: ReactNode;
  sortable?: boolean;
  sortFn?: (a: T, b: T) => number;
}

interface TextOpts<T> {
  width?: string;
  align?: Align;
  padding?: string;
  sortable?: boolean;
  sortFn?: (a: T, b: T) => number;
}

interface RankChangeOpts {
  name?: string;
  width?: string;
  padding?: string;
}

interface ColumnHelpers<T> {
  position: (
    selector: (row: T, index: number) => number | null | undefined,
    opts?: PositionOpts,
  ) => TableColumn<T>;
  rider: (
    selector: (row: T) => Rider | null | undefined,
    opts?: RiderOpts<T>,
  ) => TableColumn<T>;
  text: (
    name: ReactNode,
    selector: (row: T, index: number) => ReactNode,
    opts?: TextOpts<T>,
  ) => TableColumn<T>;
  rankChange: (
    selector: (row: T) => number | string | null | undefined,
    opts?: RankChangeOpts,
  ) => TableColumn<T>;
}

function renderRankChange(change: number | string | null | undefined): ReactNode {
  if (change == null || change === 0 || change === "") return "-";
  if (typeof change === "number") {
    const dir = change > 0 ? "up" : "down";
    return (
      <span className={`rank-change-${dir}`}>
        {change > 0 ? "▲" : "▼"}
        {Math.abs(change)}
      </span>
    );
  }
  if (change.startsWith("▲")) return <span className="rank-change-up">{change}</span>;
  if (change.startsWith("▼")) return <span className="rank-change-down">{change}</span>;
  return change;
}

function makeHelpers<T>(): ColumnHelpers<T> {
  return {
    position: (selector, opts = {}) => ({
      name: opts.name ?? "Positie",
      width: opts.width ?? "70px",
      align: opts.align,
      padding: opts.padding,
      cell: (row, index) => {
        const v = selector(row, index);
        if (v == null || v === 0) return "";
        return opts.ordinal ? `${v}e` : v;
      },
    }),
    rider: (selector, opts = {}) => ({
      name: opts.name ?? "Renner",
      width: opts.width ?? "180px",
      cell: (row) => {
        const rider = selector(row);
        if (rider == null) return opts.fallback ?? "";
        return <RiderLink rider={rider} kopman={opts.kopman?.(row)} />;
      },
      sortable: opts.sortable,
      sortFn:
        opts.sortFn ??
        ((a, b) => {
          const ra = selector(a);
          const rb = selector(b);
          if (!ra || !rb) return 0;
          return ra.lastname.localeCompare(rb.lastname);
        }),
    }),
    text: (name, selector, opts = {}) => ({
      name,
      width: opts.width,
      align: opts.align,
      padding: opts.padding,
      cell: selector,
      sortable: opts.sortable,
      sortFn: opts.sortFn,
    }),
    rankChange: (selector, opts = {}) => ({
      name: opts.name ?? "",
      width: opts.width ?? "50px",
      align: "center",
      padding: opts.padding,
      cell: (row) => renderRankChange(selector(row)),
    }),
  };
}

const PAGE_SIZE = 20;

const IconFirst = () => (
  <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" aria-hidden>
    <path d="M18.41 16.59L13.82 12l4.59-4.59L17 6l-6 6 6 6zM6 6h2v12H6z" />
  </svg>
);
const IconLast = () => (
  <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" aria-hidden>
    <path d="M5.59 7.41L10.18 12l-4.59 4.59L7 18l6-6-6-6zM16 6h2v12h-2z" />
  </svg>
);
const IconPrev = () => (
  <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" aria-hidden>
    <path d="M15.41 7.41L14 6l-6 6 6 6 1.41-1.41L10.83 12z" />
  </svg>
);
const IconNext = () => (
  <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" aria-hidden>
    <path d="M10 6L8.59 7.41 13.17 12l-4.58 4.59L10 18l6-6z" />
  </svg>
);

type KeyField<T> = keyof T | ((row: T) => string | number);

interface Props<T> {
  data: T[];
  children: (col: ColumnHelpers<T>) => TableColumn<T>[];
  title?: ReactNode;
  keyField?: KeyField<T>;
  rowClassName?: (row: T) => string | undefined;
  paginated?: boolean;
  noHead?: boolean;
  pointerOnHover?: boolean;
  expandedContent?: (row: T) => ReactNode;
}

type SortState = { col: number; dir: "asc" | "desc" } | null;
type Key = string | number;

function getKey<T>(row: T, keyField: KeyField<T> | undefined, fallback: number): Key {
  if (keyField == null) return fallback;
  if (typeof keyField === "function") return keyField(row);
  return row[keyField] as unknown as Key;
}

function Table<T>({
  data,
  children,
  title,
  keyField,
  rowClassName,
  paginated,
  noHead,
  pointerOnHover,
  expandedContent,
}: Props<T>) {
  const columns = children(makeHelpers<T>());
  const [page, setPage] = useState(0);
  const [sort, setSort] = useState<SortState>(null);
  const [expanded, setExpanded] = useState<Set<Key>>(new Set());

  const toggleExpand = (key: Key, e: React.MouseEvent) => {
    if ((e.target as HTMLElement).closest("button, a")) return;
    setExpanded((curr) => {
      const next = new Set(curr);
      if (next.has(key)) next.delete(key);
      else next.add(key);
      return next;
    });
  };

  const sortedData = (() => {
    if (!sort) return data;
    const fn = columns[sort.col]?.sortFn;
    if (!fn) return data;
    const out = [...data].sort(fn);
    return sort.dir === "desc" ? out.reverse() : out;
  })();

  const totalPages = paginated ? Math.max(1, Math.ceil(sortedData.length / PAGE_SIZE)) : 1;
  const currentPage = Math.min(page, totalPages - 1);
  const visibleRows = paginated
    ? sortedData.slice(currentPage * PAGE_SIZE, currentPage * PAGE_SIZE + PAGE_SIZE)
    : sortedData;
  const showPagination = paginated && sortedData.length > PAGE_SIZE;

  const toggleSort = (colIdx: number) =>
    setSort((curr) => {
      if (curr?.col !== colIdx) return { col: colIdx, dir: "asc" };
      if (curr.dir === "asc") return { col: colIdx, dir: "desc" };
      return null;
    });

  return (
    <table className={`sre-table${pointerOnHover ? " sre-pointer" : ""}`}>
      {title != null && <caption>{title}</caption>}
      <colgroup>
        {columns.map((c, i) => (
          <col key={i} style={c.width ? { width: c.width } : undefined} />
        ))}
      </colgroup>
      {!noHead && (
        <thead>
          <tr>
            {columns.map((c, i) => (
              <th
                key={i}
                style={{ textAlign: c.align, padding: c.padding }}
                className={c.sortable ? "sortable" : undefined}
                onClick={c.sortable ? () => toggleSort(i) : undefined}
              >
                {c.name}
                {sort?.col === i && (
                  <span className="sort-indicator"> {sort.dir === "asc" ? "▲" : "▼"}</span>
                )}
              </th>
            ))}
          </tr>
        </thead>
      )}
      <tbody>
        {visibleRows.flatMap((row, rowIdx) => {
          const dataIdx = paginated ? currentPage * PAGE_SIZE + rowIdx : rowIdx;
          const key = getKey(row, keyField, dataIdx);
          const dataRow = (
            <tr
              key={key}
              className={rowClassName?.(row)}
              onClick={expandedContent ? (e) => toggleExpand(key, e) : undefined}
            >
              {columns.map((c, i) => (
                <td key={i} style={{ textAlign: c.align, padding: c.padding }}>
                  {c.cell(row, dataIdx)}
                </td>
              ))}
            </tr>
          );
          if (!expandedContent || !expanded.has(key)) return [dataRow];
          return [
            dataRow,
            <tr key={`${key}-exp`} className="sre-table-expanded">
              <td colSpan={columns.length}>{expandedContent(row)}</td>
            </tr>,
          ];
        })}
      </tbody>
      {showPagination && (
        <tfoot>
          <tr>
            <td colSpan={columns.length}>
              <div className="sre-table-pagination">
                <span>
                  {currentPage * PAGE_SIZE + 1}–
                  {Math.min((currentPage + 1) * PAGE_SIZE, sortedData.length)} van{" "}
                  {sortedData.length}
                </span>
                <button
                  type="button"
                  disabled={currentPage === 0}
                  onClick={() => setPage(0)}
                  aria-label="Eerste pagina"
                >
                  <IconFirst />
                </button>
                <button
                  type="button"
                  disabled={currentPage === 0}
                  onClick={() => setPage(currentPage - 1)}
                  aria-label="Vorige pagina"
                >
                  <IconPrev />
                </button>
                <button
                  type="button"
                  disabled={currentPage >= totalPages - 1}
                  onClick={() => setPage(currentPage + 1)}
                  aria-label="Volgende pagina"
                >
                  <IconNext />
                </button>
                <button
                  type="button"
                  disabled={currentPage >= totalPages - 1}
                  onClick={() => setPage(totalPages - 1)}
                  aria-label="Laatste pagina"
                >
                  <IconLast />
                </button>
              </div>
            </td>
          </tr>
        </tfoot>
      )}
    </table>
  );
}

export default Table;
