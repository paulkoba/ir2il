define dso_local noundef i32 @main() #0 !dbg !10 {
  %1 = alloca i32, align 4
  %2 = alloca i32, align 4
  %3 = alloca i32, align 4
  store i32 0, ptr %1, align 4
  call void @llvm.dbg.declare(metadata ptr %2, metadata !16, metadata !DIExpression()), !dbg !17
  store i32 1, ptr %2, align 4, !dbg !17
  call void @llvm.dbg.declare(metadata ptr %3, metadata !18, metadata !DIExpression()), !dbg !19
  store i32 -3, ptr %3, align 4, !dbg !19
  br label %4, !dbg !20

4:                                                ; preds = %7, %0
  %5 = load i32, ptr %2, align 4, !dbg !21
  %6 = icmp ne i32 %5, 10, !dbg !22
  br i1 %6, label %7, label %13, !dbg !20

7:                                                ; preds = %4
  %8 = load i32, ptr %2, align 4, !dbg !23
  %9 = load i32, ptr %3, align 4, !dbg !25
  %10 = add nsw i32 %9, %8, !dbg !25
  store i32 %10, ptr %3, align 4, !dbg !25
  %11 = load i32, ptr %2, align 4, !dbg !26
  %12 = add nsw i32 %11, 1, !dbg !26
  store i32 %12, ptr %2, align 4, !dbg !26
  br label %4, !dbg !20, !llvm.loop !27

13:                                               ; preds = %4
  %14 = load i32, ptr %3, align 4, !dbg !30
  ret i32 %14, !dbg !31
}

declare void @llvm.dbg.declare(metadata, metadata, metadata) #1

attributes #0 = { mustprogress noinline norecurse nounwind optnone uwtable "frame-pointer"="all" "min-legal-vector-width"="0" "no-trapping-math"="true" "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+cx8,+fxsr,+mmx,+sse,+sse2,+x87" "tune-cpu"="generic" }
attributes #1 = { nocallback nofree nosync nounwind speculatable willreturn memory(none) }
