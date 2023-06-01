define dso_local noundef i32 @main() #0 !dbg !10 {
  %1 = alloca i32, align 4
  store i32 0, ptr %1, align 4
  ret i32 42, !dbg !16
}

attributes #0 = { mustprogress noinline norecurse nounwind optnone uwtable "frame-pointer"="all" "min-legal-vector-width"="0" "no-trapping-math"="true" "stack-protector-buffer-size"="8" "target-cpu"="x86-64" "target-features"="+cx8,+fxsr,+mmx,+sse,+sse2,+x87" "tune-cpu"="generic" }
