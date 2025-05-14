using System;
using System.Collections.Generic;

namespace GUIGUI17F.ProtobufManager
{
    /// <summary>
    /// manager for custom undo/redo logic
    /// </summary>
    public class CustomUndoRedoManager
    {
        private const int MaxStackDeep = 20;

        public static CustomUndoRedoManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CustomUndoRedoManager();
                }
                return _instance;
            }
        }

        private static CustomUndoRedoManager _instance;

        private List<Action> _undoStack;
        private List<Action> _redoStack;

        private CustomUndoRedoManager()
        {
            _undoStack = new List<Action>();
            _redoStack = new List<Action>();
        }

        public void RecordForUndo(Action action)
        {
            if (_undoStack.Count >= MaxStackDeep)
            {
                _undoStack.RemoveAt(0);
            }
            _undoStack.Add(action);
        }

        public void RecordForRedo(Action action)
        {
            if (_redoStack.Count >= MaxStackDeep)
            {
                _redoStack.RemoveAt(0);
            }
            _redoStack.Add(action);
        }

        public void PerformUndo()
        {
            if (_undoStack.Count > 0)
            {
                int index = _undoStack.Count - 1;
                Action action = _undoStack[index];
                _undoStack.RemoveAt(index);
                action?.Invoke();
            }
        }

        public void PerformRedo()
        {
            if (_redoStack.Count > 0)
            {
                int index = _redoStack.Count - 1;
                Action action = _redoStack[index];
                _redoStack.RemoveAt(index);
                action?.Invoke();
            }
        }
    }
}