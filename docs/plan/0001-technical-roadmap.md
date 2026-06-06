# Technical Roadmap

- Статус: Proposed
- Цель: получить проверяемый voice-first vertical slice до расширения overlay и ML

## Согласованное техническое решение

| Область | Начальный выбор | Почему |
|---|---|---|
| Runtime | `.NET 10 LTS`, C# 14 на Windows | Новый проект, LTS, актуальный runtime и язык |
| Telemetry | `SVappsLAB.iRacingTelemetrySDK` за собственным adapter | Live + Session Info + `.ibt`, Apache-2.0 |
| Strategy | Собственное pure deterministic ядро | Числа тестируемы и не зависят от LLM |
| Audio capture | NAudio / WASAPI | Зрелая Windows/.NET интеграция |
| VAD | Silero VAD / ONNX Runtime | Offline, быстрый, снижает STT hallucinations |
| STT | Whisper.net + `small multilingual` | Локальный RU/EN и свободные формулировки |
| TTS | sherpa-onnx + проверенный Piper/VITS voice | Локальный CPU inference |
| TTS fallback | Windows SpeechSynthesizer | Дешевый надежный fallback |
| Interaction | push-to-talk | Предсказуемость в гоночном шуме |
| UI | Минимальный WPF overlay | Голос является основным интерфейсом |
| Persistence | Сначала replay fixtures; SQLite/Parquet после spike | Не закреплять формат до проверки данных |

## Целевая структура solution

```text
src/
  VoiceRaceEngineer.Domain/
  VoiceRaceEngineer.Strategy/
  VoiceRaceEngineer.Telemetry.Abstractions/
  VoiceRaceEngineer.Telemetry.IRacing/
  VoiceRaceEngineer.Voice.Abstractions/
  VoiceRaceEngineer.Voice.Local/
  VoiceRaceEngineer.Orchestration/
  VoiceRaceEngineer.App/
tests/
  VoiceRaceEngineer.Strategy.Tests/
  VoiceRaceEngineer.Telemetry.Tests/
  VoiceRaceEngineer.Voice.Benchmarks/
  VoiceRaceEngineer.Replay.Tests/
fixtures/
  telemetry/
  voice/
```

## Этап 0: Подготовка среды

Результат:

- установлен `.NET 10 SDK` на dev и Windows test machine;
- `global.json` фиксирует SDK;
- создан solution и CI для pure cross-platform tests;
- C# 14, nullable, analyzers, warnings-as-errors и deterministic builds включены централизованно;
- версии NuGet управляются через Central Package Management;
- Windows-only проекты отделены от доменного ядра;
- лицензии зависимостей и моделей фиксируются отдельным manifest.

На текущем Mac установлен `.NET 10.0.108` и создается кроссплатформенное ядро. Windows-интеграция и WPF проверяются отдельно на Windows test machine.

## Этап 1: Pure Strategy Core

Сначала реализовать без iRacing и без голоса:

- unit types: liters, liters-per-lap, laps, seconds;
- lap-limited remaining distance;
- fuel-to-finish;
- extra-lap и K-lap saving;
- structured result, assumptions, warnings и calculation trace;
- unit, boundary и property-based tests.

Критерий готовности:

- идентичные inputs дают идентичные outputs;
- все формулы покрыты boundary tests;
- LLM не участвует.

## Этап 2: Telemetry Spike

На Windows с iRacing:

- подключить SVappsLAB SDK за `IIracingTelemetrySource`;
- прочитать минимальный набор telemetry variables;
- нормализовать Race State;
- проверить reconnect, session changes и dropped frames;
- записать `.ibt` и воспроизвести через тот же adapter.

Критерий готовности:

- live и replay создают одинаковые normalized snapshots;
- Strategy Engine не блокирует telemetry reader;
- семантика lap/time/pit переменных задокументирована реальными наблюдениями.

## Этап 3: Replayable Strategy State

- Strategy State Reducer;
- классификация green/caution/pit/refuel/reset laps;
- robust estimator через median/MAD и recency weighting;
- deterministic confidence;
- golden replay fixtures.

Критерий готовности:

- fuel prediction воспроизводится по записи;
- rejected laps объяснимы;
- engine умеет отвечать `Unavailable`.

## Этап 4: Local Voice Spike

- NAudio/WASAPI capture;
- push-to-talk;
- Silero VAD;
- Whisper.net `base/small/medium` benchmark;
- sherpa-onnx/Piper и Windows TTS benchmark;
- interruptible playback;
- запрет STT во время собственного TTS.

Критерий готовности:

- intent accuracy в игровом шуме `>= 90%`;
- STT p95 `< 900 ms`;
- TTS time-to-first-audio p95 `< 350 ms`;
- потеря FPS `< 2%`;
- выбраны модели и проверены их лицензии.

## Этап 5: Voice Vertical Slice

Поддержать четыре typed tools:

- `get_fuel_status`;
- `get_fuel_to_finish`;
- `get_extra_lap_saving`;
- `evaluate_fuel_scenario`.

Первый демонстрационный диалог:

1. «Хватит ли топлива до финиша?»
2. «А если проехать еще один круг?»
3. «Сколько экономить следующие пять кругов?»
4. «Это реально с моим текущим расходом?»

Критерий готовности:

- follow-up сохраняет контекст;
- 100% озвученных чисел приходят из Strategy Engine;
- ответ показывает snapshot ID/confidence в debug overlay;
- простой ответ начинается менее чем через 2 секунды.

## Этап 6: Timed Race Validation

- модель финиша по overall leader;
- multiclass;
- time-and-lap limits;
- white/checkered flags;
- fallback при отсутствующих данных.

Критерий готовности:

- прогноз финального круга проверен на реальных replay fixtures;
- ошибки и confidence измеряются по кругам.

## Этап 7: Shadow Mode

В реальной сессии приложение считает стратегию, но не дает активных рекомендаций:

- сравнивает predicted и actual finish fuel;
- фиксирует ошибку по кругам;
- проверяет ложные советы;
- измеряет latency и влияние на FPS.

Только после shadow validation включаются рекомендации.

## Что не входит в первые этапы

- универсальный набор overlay widgets;
- ML-прогноз пит-стопов соперников;
- wake word и always-on listening;
- автоматические pit commands;
- облачный сбор telemetry;
- использование в Ranked/Official без письменного подтверждения iRacing.

## Основные риски

| Риск | Контроль |
|---|---|
| Ошибочная семантика SDK | Raw values + replay fixtures + Windows spike |
| Неверные числа от LLM | Typed tools, structured results, calculation trace |
| STT ошибается в шуме | PTT, VAD, intent benchmark, overlay confirmation |
| Голосовая модель имеет ограничения лицензии | Model manifest и проверка каждого checkpoint |
| Voice inference влияет на FPS | Benchmark на целевом ПК и resource budgets |
| Timed race дает лишний круг | Отдельная validation stage и conservative fallback |
| SDK-библиотека перестала подходить | Собственный adapter boundary |

## Первый рабочий milestone

`M1: Replay Fuel Engineer`

На вход получает replay fixture, на выход возвращает и озвучивает:

- устойчивый расход;
- оставшуюся дистанцию;
- fuel margin;
- экономию для дополнительного круга;
- confidence и assumptions.

Этот milestone можно разрабатывать и тестировать без запущенного iRacing, а затем подключить live adapter.

## Связанные документы

- [ADR-0001](../adr/0001-local-ai-race-engineer-overlay.md)
- [Agent execution plan](0002-agent-execution-plan.md)
- [C#/.NET iRacing SDK libraries](../research/0002-dotnet-iracing-sdk-libraries.md)
- [Local STT/TTS stack](../research/0003-local-stt-tts-stack.md)
- [Strategy Engine architecture](../architecture/0001-strategy-engine.md)
