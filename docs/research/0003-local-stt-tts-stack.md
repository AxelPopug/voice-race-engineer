# Исследование локального STT/TTS стека

- Дата проверки: 2026-06-06
- Целевая среда: Windows, `.NET 8`, русский и английский
- Статус решения: кандидаты выбраны, требуется benchmark на целевом игровом ПК

## Решение для MVP

```text
NAudio / WASAPI
+ push-to-talk
+ Silero VAD through ONNX Runtime
+ Whisper.net / whisper.cpp small multilingual
+ sherpa-onnx with Piper/VITS voices
+ Windows SpeechSynthesizer fallback
```

Все модели загружаются при старте. Аудио по умолчанию не покидает компьютер. Для критичных коротких сообщений допускаются заранее синтезированные фразы.

## Целевые требования

| Требование | Цель |
|---|---:|
| Языки | русский и английский |
| Работа без интернета | обязательна |
| STT после отпускания PTT, p95 | `< 900 ms` |
| TTS time-to-first-audio, p95 | `< 350 ms` |
| Начало полного ответа на простой запрос | `< 2 s` |
| Intent accuracy в гоночном шуме | `>= 90%` |
| Точность чисел в распознанном вопросе | `>= 95%` |
| Потеря FPS | `< 2%` |

## STT-кандидаты

| Решение | RU / EN | Streaming | Runtime | .NET | Лицензия | Решение |
|---|---|---|---|---|---|---|
| [Whisper.net](https://github.com/sandrohanea/whisper.net) / [whisper.cpp](https://github.com/ggml-org/whisper.cpp) | Хорошо | Chunked | CPU, CUDA, Vulkan, OpenVINO | Native NuGet | MIT | Основной MVP |
| [faster-whisper](https://github.com/SYSTRAN/faster-whisper) | Хорошо | Требует обвязки | CPU, NVIDIA CUDA | Python sidecar | MIT | Benchmark-контроль |
| [sherpa-onnx](https://github.com/k2-fsa/sherpa-onnx) ASR | Зависит от модели | Настоящий | ONNX providers | Официальные bindings | Apache-2.0, модели отдельно | Возможный быстрый backend |
| [Vosk](https://github.com/alphacep/vosk-api) | Да, раздельные модели | Настоящий | CPU | C# bindings | Apache-2.0 | Fallback для фиксированных команд |
| Windows Speech Recognition | Зависит от Windows | Да | Системный | WinRT | Windows API | Только fallback |

### Почему Whisper.net

- хорошо соответствует свободным формулировкам;
- одна multilingual-модель покрывает RU/EN;
- полностью локальная интеграция без Python-sidecar;
- initial prompt позволяет подсказать гоночные термины;
- push-to-talk снимает необходимость настоящего streaming.

Основные риски:

- hallucinations на тишине и шуме;
- задержка на слабом CPU;
- модели `medium` и крупнее могут мешать FPS;
- качество чисел нужно измерять отдельно от общего WER.

Для первого benchmark сравнить `base`, `small` и `medium`. Начальный кандидат — `small multilingual`.

## VAD и захват аудио

Использовать `NAudio` с WASAPI, mono PCM `16 kHz`. Даже при push-to-talk применять [Silero VAD](https://github.com/snakers4/silero-vad):

- удаляет тишину;
- снижает hallucinations Whisper;
- помогает определить конец фразы;
- работает локально через ONNX Runtime;
- имеет MIT-лицензию.

Во время собственного TTS распознавание должно быть выключено, иначе помощник начнет слушать себя.

## TTS-кандидаты

| Решение | RU / EN | Runtime | .NET | Лицензия | Решение |
|---|---|---|---|---|---|
| Piper/VITS через sherpa-onnx | Да, зависит от голоса | CPU, ONNX | Официальные bindings | Runtime и каждый голос проверять отдельно | Основной MVP |
| Windows `SpeechSynthesizer` | Если голос установлен | Системный | WinRT | Windows API | Надежный fallback |
| [Kokoro-82M](https://huggingface.co/hexgrad/Kokoro-82M) | В основном EN | CPU/GPU | Через runtime/обвязку | Apache-2.0 | После MVP для EN |
| Silero TTS | Сильный RU | PyTorch | Требует обвязки | Проверять модель | Benchmark после MVP |

### Лицензионное правило

Лицензия runtime не гарантирует допустимость конкретной модели или голоса. Для каждого поставляемого checkpoint хранить:

- источник;
- версию или hash;
- лицензию;
- разрешенное использование;
- attribution requirements.

## Абстракции

```csharp
public interface ISpeechRecognizer
{
    Task<RecognitionResult> RecognizeAsync(
        ReadOnlyMemory<float> audio,
        string? language,
        CancellationToken cancellationToken);
}

public interface ISpeechSynthesizer
{
    IAsyncEnumerable<AudioChunk> SynthesizeAsync(
        string text,
        string language,
        CancellationToken cancellationToken);
}
```

Backend выбирается конфигурацией. Voice Orchestrator не зависит от конкретной модели.

## Benchmark plan

Записать минимум 200 команд на язык, минимум от трех дикторов. Включить свободные формулировки, гоночные термины, фамилии, числа, дроби и единицы измерения.

Смешать команды с реальным игровым звуком:

- чистая речь;
- шум `0 dB`;
- шум `-5 dB`;
- шум `-10 dB`;
- шум плюс Discord или spotter.

Метрики STT:

- WER/CER;
- intent accuracy;
- точность чисел и единиц;
- hallucination rate на одном шуме;
- p50/p95/p99 latency после PTT;
- CPU/GPU/RAM/VRAM;
- FPS и frame time iRacing.

Метрики TTS:

- time-to-first-audio;
- real-time factor;
- понятность чисел в игровом шуме;
- возможность немедленного прерывания;
- субъективная естественность;
- использование ресурсов.

## Обязательные UX-ограничения

- push-to-talk в MVP;
- headset microphone как рекомендуемый режим;
- отображение распознанного вопроса в overlay;
- возможность быстро отменить неверно распознанный запрос;
- glossary с гоночными терминами;
- interruptible TTS;
- запрет слушать собственный TTS;
- noise suppression добавляется только после A/B benchmark.

## Источники

- [Whisper](https://github.com/openai/whisper)
- [whisper.cpp](https://github.com/ggml-org/whisper.cpp)
- [Whisper.net](https://github.com/sandrohanea/whisper.net)
- [faster-whisper](https://github.com/SYSTRAN/faster-whisper)
- [sherpa-onnx](https://github.com/k2-fsa/sherpa-onnx)
- [Vosk API](https://github.com/alphacep/vosk-api)
- [Silero VAD](https://github.com/snakers4/silero-vad)
- [Windows speech recognition](https://learn.microsoft.com/en-us/windows/apps/design/input/speech-recognition)
- [Windows SpeechSynthesizer](https://learn.microsoft.com/en-us/uwp/api/windows.media.speechsynthesis.speechsynthesizer)
- [Piper voices](https://github.com/OHF-Voice/piper1-gpl/blob/main/docs/VOICES.md)
- [Piper voice models](https://huggingface.co/rhasspy/piper-voices)
