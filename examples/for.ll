
define dso_local noundef i32 @main() #0 !dbg !10 {
  %1 = alloca i32, align 4
  %2 = alloca i32, align 4
  %3 = alloca i32, align 4
  store i32 0, ptr %1, align 4
  call void @llvm.dbg.declare(metadata ptr %2, metadata !16, metadata !DIExpression()), !dbg !17
  store i32 0, ptr %2, align 4, !dbg !17
  call void @llvm.dbg.declare(metadata ptr %3, metadata !18, metadata !DIExpression()), !dbg !20
  store i32 0, ptr %3, align 4, !dbg !20
  br label %4, !dbg !21

4:                                                ; preds = %11, %0
  %5 = load i32, ptr %3, align 4, !dbg !22
  %6 = icmp slt i32 %5, 10, !dbg !24
  br i1 %6, label %7, label %14, !dbg !25

7:                                                ; preds = %4
  %8 = load i32, ptr %3, align 4, !dbg !26
  %9 = load i32, ptr %2, align 4, !dbg !28
  %10 = add nsw i32 %9, %8, !dbg !28
  store i32 %10, ptr %2, align 4, !dbg !28
  br label %11, !dbg !29

11:                                               ; preds = %7
  %12 = load i32, ptr %3, align 4, !dbg !30
  %13 = add nsw i32 %12, 1, !dbg !30
  store i32 %13, ptr %3, align 4, !dbg !30
  br label %4, !dbg !31, !llvm.loop !32

14:                                               ; preds = %4
  %15 = load i32, ptr %2, align 4, !dbg !35
  ret i32 %15, !dbg !36
}

declare void @llvm.dbg.declare(metadata, metadata, metadata) #1

attributes #0 = { mustprogress noinline norecurse nounwind optnone uwtable "frame-pointer"="all" "min-legal-vector-width"="0" "no-trapping-math"="true" "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+cx8,+fxsr,+mmx,+sse,+sse2,+x87" "tune-cpu"="generic" }
attributes #1 = { nocallback nofree nosync nounwind speculatable willreturn memory(none) }
