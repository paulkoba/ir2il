define dso_local noundef i32 @main() #0 !dbg !10 {
  %1 = alloca i32, align 4
  %2 = alloca i32, align 4
  store i32 0, ptr %1, align 4
  call void @llvm.dbg.declare(metadata ptr %2, metadata !16, metadata !DIExpression()), !dbg !17
  store i32 1, ptr %2, align 4, !dbg !17
  %3 = load i32, ptr %2, align 4, !dbg !18
  %4 = icmp slt i32 %3, 42, !dbg !20
  br i1 %4, label %5, label %6, !dbg !21

5:                                                ; preds = %0
  br label %7, !dbg !22

6:                                                ; preds = %0
  br label %7

7:                                                ; preds = %6, %5
  ret i32 43, !dbg !24
}

declare void @llvm.dbg.declare(metadata, metadata, metadata) #1

attributes #0 = { mustprogress noinline norecurse nounwind optnone uwtable "frame-pointer"="all" "min-legal-vector-width"="0" "no-trapping-math"="true" "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+cx8,+fxsr,+mmx,+sse,+sse2,+x87" "tune-cpu"="generic" }
attributes #1 = { nocallback nofree nosync nounwind speculatable willreturn memory(none) }