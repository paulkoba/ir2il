define dso_local noundef i32 @_Z6squareiii(i32 noundef %0, i32 noundef %1, i32 noundef %2) local_unnamed_addr #0 !dbg !9 {
  call void @llvm.dbg.value(metadata i32 %0, metadata !15, metadata !DIExpression()), !dbg !20
  call void @llvm.dbg.value(metadata i32 %1, metadata !16, metadata !DIExpression()), !dbg !20
  call void @llvm.dbg.value(metadata i32 %2, metadata !17, metadata !DIExpression()), !dbg !20
  %4 = add nsw i32 %1, %0, !dbg !21
  call void @llvm.dbg.value(metadata i32 %4, metadata !18, metadata !DIExpression()), !dbg !20
  %5 = sub nsw i32 %2, %0, !dbg !22
  call void @llvm.dbg.value(metadata i32 %5, metadata !19, metadata !DIExpression()), !dbg !20
  %6 = mul nsw i32 %5, %4, !dbg !23
  %7 = sub nsw i32 %6, %1, !dbg !24
  ret i32 %7, !dbg !25
}

define dso_local noundef i32 @main() local_unnamed_addr #0 !dbg !26 {
  ret i32 4, !dbg !30
}

declare void @llvm.dbg.value(metadata, metadata, metadata) #1

attributes #0 = { mustprogress nofree norecurse nosync nounwind willreturn memory(none) uwtable "min-legal-vector-width"="0" "no-trapping-math"="true" "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+cx8,+fxsr,+mmx,+sse,+sse2,+x87" "tune-cpu"="generic" }
attributes #1 = { nocallback nofree nosync nounwind speculatable willreturn memory(none) }
