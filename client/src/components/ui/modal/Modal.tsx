import { useRef, useEffect } from "react";
import "./modal.css";

const Modal = ({
  open,
  closeFn,
  modalContents,
}: {
  open: boolean;
  closeFn: () => void;
  modalContents: JSX.Element;
}) => {
  const dialogRef = useRef<HTMLDialogElement | null>(null);
  useEffect(() => {
    if (open) {
      dialogRef.current?.showModal();
    } else {
      dialogRef.current?.close();
    }
  });
  return (
    <dialog ref={dialogRef}>
      <div className="modal-header">
        <button className="modal-close-button" onClick={closeFn}>
          ✖
        </button>
      </div>
      {modalContents}
    </dialog>
  );
};

export default Modal;
