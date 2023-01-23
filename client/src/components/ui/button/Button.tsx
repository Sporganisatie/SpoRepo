import './button.css';

type ButtonType = "";

interface Buttonprops {
    action: () => {};
    type: ButtonType;
    label: string;
    disabled?: boolean;
    icon?: string;
    size?: string;
}

const Button = (props: Buttonprops) => {
    return (
      <button 
        className="button blue" 
        onClick={props.action}
        disabled={props.disabled ?? false}>
        {props.label}
      </button>
    )
  }
  
  export default Button
  