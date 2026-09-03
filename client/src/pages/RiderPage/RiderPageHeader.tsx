import { formatPrice } from "@/lib/format";

interface RiderPageHeaderProps {
    firstname: string;
    lastname: string;
    price?: number;
    tooExpensive?: boolean;
    dnf?: boolean;
}

const RiderPageHeader = ({ firstname, lastname, price, tooExpensive, dnf }: RiderPageHeaderProps) => (
    <div className="panel rider-page-header">
        <div className="panel-header">
            <h2 className="panel-title">
                {firstname} {lastname}
            </h2>
            {price !== undefined && (
                <span className="panel-meta">
                    Prijs: {formatPrice(price)}
                    {tooExpensive && <span className="rider-race-too-expensive"> · Te duur voor budget</span>}
                    {dnf && " · DNF"}
                </span>
            )}
        </div>
    </div>
);

export default RiderPageHeader;
