using System.Collections;
using System.Collections.Generic;
using TUIOsharp.DataProcessors;
using TUIOsharp;
using UnityEngine;
using TUIOsharp.Entities;
using System;
using UnityEngine.VFX;
using Unity.VisualScripting;

public class MyTUIOCursor : MonoBehaviour
{
    private TuioServer _tuioServer;
    private Dictionary<Int32, TuioCursor> _cursorList = null;
    [SerializeField] VisualEffect effect;
    private Dictionary<Int32, VisualEffect> effectList = null;
    public int scaleSize = 10;

    void Start()
    {
        _cursorList = new Dictionary<Int32, TuioCursor>();
        _tuioServer = new TuioServer();
        effectList = new Dictionary<int, VisualEffect>();
        var cursorProcessor = new CursorProcessor();

        // �V�������������_�f�[�^����M
        // �J�[�\������ID�ƕR�Â��Ă���
        cursorProcessor.CursorAdded += (sender, e) =>
        {
            var entity = e.Cursor;
            lock (_tuioServer) { _cursorList.Add(entity.Id, entity); }
        };

        // ���S�����_�f�[�^����M
        // ���SID���폜
        cursorProcessor.CursorRemoved += (sender, e) =>
        {
            var entity = e.Cursor;
            lock (_tuioServer) { _cursorList.Remove(entity.Id); }
        };

        // �ʒu�̍X�V�A�J�[�\���̃��X�g�͕ύX���Ȃ��Ă�����
        cursorProcessor.CursorUpdated += (sender, e) => { };

        _tuioServer.Connect();
        _tuioServer.AddDataProcessor(cursorProcessor);
    }

    private void Update()
    {
        foreach (var key in _cursorList.Keys)
        {
            _cursorList.TryGetValue(key, out var cursor);
            var p = new Vector3(cursor.X * scaleSize, cursor.Y * scaleSize, 0);
            Debug.Log($"Key {key} {cursor.X} {cursor.Y}");

            if (!effectList.ContainsKey(key))
            {
                var tmpObject = Instantiate(effect, p, Quaternion.identity);
                effectList.Add(key, tmpObject);
            }
            else
            {
                effectList[key].transform.position = p;
            }
        }

        foreach(var key in effectList.Keys)
        {
            if (!_cursorList.ContainsKey(key))
            {
                Destroy(effectList[key].gameObject);
                effectList.Remove(key);
            }
                
        }
    }
}
