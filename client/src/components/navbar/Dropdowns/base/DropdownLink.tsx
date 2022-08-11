import { Link } from "react-router-dom";

interface DropdownLinkProps {
    url: string,
    title: string
}

const DropdownLink = (props: DropdownLinkProps) => {
    return (
        <Link className="navbar_dropdown_item" to={props.url}>
            {props.title}
        </Link>
    )
}

export { DropdownLink, DropdownLinkProps }
