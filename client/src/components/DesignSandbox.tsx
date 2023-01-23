import Button from "./ui/button/Button";

const DesignSandbox = () => {
  return (
    <div style={{paddingLeft: "50px"}}>
      <Button
        action={()=>{return 0;}}
        type=''
        label='Button'
      />
    </div>
  );
};

export default DesignSandbox;
