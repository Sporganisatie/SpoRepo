import './button.css';

type ButtonType = "button" | "submit" | "reset";

type ButtonClass =  "";

interface Buttonprops {
    action?: () => {};
    buttonClass?: ButtonClass;
    disabled?: boolean;
    icon?: string;
    label?: string;
    size?: string;
    type?: ButtonType;
}

const Button = (props: Buttonprops) => {
    return (
      <button 
        className="button blue" 
        onClick={props.action}
        disabled={props.disabled ?? false}
        type={props.type ?? "button"}>
        {props.label}
      </button>
    )
  }
  
  export default Button
  