import { Row } from "../components/Table/Table";

export interface Rider extends Row {
    firstName: string;
    lastName: string;
    initials: string;
    country: string;
    riderId: number;
}