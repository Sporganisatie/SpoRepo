import { useRef, useEffect } from "react";
import "./modal.css";

const Modal = ({
  open,
  closeFn,
  modalContents,
  title,
}: {
  open: boolean;
  closeFn: () => void;
  modalContents: JSX.Element;
  title?: string;
}) => {
  const dialogRef = useRef<HTMLDialogElement | null>(null);
  if (dialogRef.current) {
    dialogRef.current.addEventListener("click", (event) => {
      const rect = dialogRef.current?.getBoundingClientRect();
      if (!rect) {
        return;
      }

      if (
        rect.left > event.clientX ||
        rect.right < event.clientX ||
        rect.top > event.clientY ||
        rect.bottom < event.clientY
      ) {
        closeFn();
      }
    });
  }
  useEffect(() => {
    if (open) {
      dialogRef.current?.showModal();
    } else {
      dialogRef.current?.close();
    }
  });
  return (
    <dialog ref={dialogRef} className="content">
      <div className="modal-header">
        <div className="modal-title">{title}</div>
        <button className="modal-close-button" onClick={closeFn}>
          ✕
        </button>
      </div>
      <div className="modal-content">{modalContents}</div>
    </dialog>
  );
};

export default Modal;
