// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

namespace Game.Config
{

using global::System;
using global::System.Collections.Generic;
using global::Google.FlatBuffers;

public struct MonsterCfg : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_23_3_3(); }
  public static MonsterCfg GetRootAsMonsterCfg(ByteBuffer _bb) { return GetRootAsMonsterCfg(_bb, new MonsterCfg()); }
  public static MonsterCfg GetRootAsMonsterCfg(ByteBuffer _bb, MonsterCfg obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public static bool VerifyMonsterCfg(ByteBuffer _bb) {Google.FlatBuffers.Verifier verifier = new Google.FlatBuffers.Verifier(_bb); return verifier.VerifyBuffer("", false, MonsterCfgVerify.Verify); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public MonsterCfg __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public Game.Config.Monster? Items(int j) { int o = __p.__offset(4); return o != 0 ? (Game.Config.Monster?)(new Game.Config.Monster()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
  public int ItemsLength { get { int o = __p.__offset(4); return o != 0 ? __p.__vector_len(o) : 0; } }

  public static Offset<Game.Config.MonsterCfg> CreateMonsterCfg(FlatBufferBuilder builder,
      VectorOffset itemsOffset = default(VectorOffset)) {
    builder.StartTable(1);
    MonsterCfg.AddItems(builder, itemsOffset);
    return MonsterCfg.EndMonsterCfg(builder);
  }

  public static void StartMonsterCfg(FlatBufferBuilder builder) { builder.StartTable(1); }
  public static void AddItems(FlatBufferBuilder builder, VectorOffset itemsOffset) { builder.AddOffset(0, itemsOffset.Value, 0); }
  public static VectorOffset CreateItemsVector(FlatBufferBuilder builder, Offset<Game.Config.Monster>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static VectorOffset CreateItemsVectorBlock(FlatBufferBuilder builder, Offset<Game.Config.Monster>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreateItemsVectorBlock(FlatBufferBuilder builder, ArraySegment<Offset<Game.Config.Monster>> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreateItemsVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<Offset<Game.Config.Monster>>(dataPtr, sizeInBytes); return builder.EndVector(); }
  public static void StartItemsVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<Game.Config.MonsterCfg> EndMonsterCfg(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<Game.Config.MonsterCfg>(o);
  }
  public static void FinishMonsterCfgBuffer(FlatBufferBuilder builder, Offset<Game.Config.MonsterCfg> offset) { builder.Finish(offset.Value); }
  public static void FinishSizePrefixedMonsterCfgBuffer(FlatBufferBuilder builder, Offset<Game.Config.MonsterCfg> offset) { builder.FinishSizePrefixed(offset.Value); }
}


static public class MonsterCfgVerify
{
  static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
  {
    return verifier.VerifyTableStart(tablePos)
      && verifier.VerifyVectorOfTables(tablePos, 4 /*Items*/, Game.Config.MonsterVerify.Verify, false)
      && verifier.VerifyTableEnd(tablePos);
  }
}

}