
define dso_local noundef i32 @_Z6squareiii(i32 noundef %0, i32 noundef %1, i32 noundef %2) #0 !dbg !10 {
  %4 = alloca i32, align 4
  %5 = alloca i32, align 4
  %6 = alloca i32, align 4
  %7 = alloca i32, align 4
  %8 = alloca i32, align 4
  store i32 %0, ptr %4, align 4
  call void @llvm.dbg.declare(metadata ptr %4, metadata !16, metadata !DIExpression()), !dbg !17
  store i32 %1, ptr %5, align 4
  call void @llvm.dbg.declare(metadata ptr %5, metadata !18, metadata !DIExpression()), !dbg !19
  store i32 %2, ptr %6, align 4
  call void @llvm.dbg.declare(metadata ptr %6, metadata !20, metadata !DIExpression()), !dbg !21
  call void @llvm.dbg.declare(metadata ptr %7, metadata !22, metadata !DIExpression()), !dbg !23
  %9 = load i32, ptr %4, align 4, !dbg !24
  %10 = load i32, ptr %5, align 4, !dbg !25
  %11 = add nsw i32 %9, %10, !dbg !26
  store i32 %11, ptr %7, align 4, !dbg !23
  call void @llvm.dbg.declare(metadata ptr %8, metadata !27, metadata !DIExpression()), !dbg !28
  %12 = load i32, ptr %6, align 4, !dbg !29
  %13 = load i32, ptr %4, align 4, !dbg !30
  %14 = sub nsw i32 %12, %13, !dbg !31
  store i32 %14, ptr %8, align 4, !dbg !28
  %15 = load i32, ptr %7, align 4, !dbg !32
  %16 = load i32, ptr %8, align 4, !dbg !33
  %17 = mul nsw i32 %15, %16, !dbg !34
  %18 = load i32, ptr %5, align 4, !dbg !35
  %19 = sub nsw i32 %17, %18, !dbg !36
  ret i32 %19, !dbg !37
}

declare void @llvm.dbg.declare(metadata, metadata, metadata) #1

define dso_local noundef i32 @main() #2 !dbg !38 {
  %1 = alloca i32, align 4
  store i32 0, ptr %1, align 4
  %2 = call noundef i32 @_Z6squareiii(i32 noundef 1, i32 noundef 2, i32 noundef 3), !dbg !41
  ret i32 %2, !dbg !42
}

attributes #0 = { mustprogress noinline nounwind optnone uwtable "frame-pointer"="all" "min-legal-vector-width"="0" "no-trapping-math"="true" "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+cx8,+fxsr,+mmx,+sse,+sse2,+x87" "tune-cpu"="generic" }
attributes #1 = { nocallback nofree nosync nounwind speculatable willreturn memory(none) }
attributes #2 = { mustprogress noinline norecurse nounwind optnone uwtable "frame-pointer"="all" "min-legal-vector-width"="0" "no-trapping-math"="true" "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+cx8,+fxsr,+mmx,+sse,+sse2,+x87" "tune-cpu"="generic" }
