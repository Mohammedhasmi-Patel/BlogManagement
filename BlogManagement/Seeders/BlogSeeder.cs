using BlogManagement.Database;
using BlogManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogManagement.Seeders
{
    public static class BlogSeeder
    {
        public static async Task<List<Blog>> SeedAsync(
            AppDbContext context,
            Dictionary<string, AppUser> users,
            Dictionary<string, Category> categories)
        {
            var seededBlogs = new List<Blog>();

            if (users.Count == 0 || categories.Count == 0)
            {
                return seededBlogs;
            }

            var sophia = users.GetValueOrDefault("sophia.turner@example.com") ?? users.Values.First();
            var liam = users.GetValueOrDefault("liam.chen@example.com") ?? sophia;
            var elena = users.GetValueOrDefault("elena.rostova@example.com") ?? sophia;

            var webDevCat = categories.GetValueOrDefault("web-development");
            var aiCat = categories.GetValueOrDefault("artificial-intelligence");
            var cloudCat = categories.GetValueOrDefault("cloud-devops");
            var uiUxCat = categories.GetValueOrDefault("ui-ux-design");
            var mobileCat = categories.GetValueOrDefault("mobile-development");
            var careerCat = categories.GetValueOrDefault("career-and-culture");

            var blogData = new[]
            {
                new
                {
                    Title = "Architecting Resilient Microservices with ASP.NET Core and Clean Architecture",
                    Slug = "architecting-resilient-microservices-aspnet-core",
                    Author = sophia,
                    Status = "published",
                    ViewCount = 1845,
                    ReadingTimeMinutes = 7,
                    PublishedDaysAgo = 14,
                    CoverImage = "https://images.unsplash.com/photo-1517694712202-14dd9538aa97?w=1200&auto=format&fit=crop&q=80",
                    Summary = "Discover how to design and build highly available, domain-driven microservices using ASP.NET Core, EF Core, RabbitMQ, and resilient fault-tolerance patterns.",
                    Categories = new[] { webDevCat, cloudCat }.Where(c => c != null).Select(c => c!).ToList(),
                    Content = @"# Architecting Resilient Microservices with ASP.NET Core

Modern enterprise systems require architectures that can scale horizontally, recover gracefully from transient faults, and evolve independently without continuous downtime.

In this guide, we break down key architectural patterns for building robust distributed systems using **ASP.NET Core** and **Clean Architecture**.

---

## 1. Domain-Driven Design (DDD) & Clean Architecture

Clean Architecture enforces a strict separation of concerns where business rules remain isolated from frameworks, databases, and UI layers.

```
+-------------------------------------------------------+
|  Presentation Layer (Controllers, Minimal APIs)       |
|    |                                                  |
|    v                                                  |
|  Application Layer (Use Cases, Commands, Queries, DTO)|
|    |                                                  |
|    v                                                  |
|  Domain Layer (Entities, Value Objects, Aggregates)   |
|    ^                                                  |
|    |                                                  |
|  Infrastructure Layer (EF Core, RabbitMQ, Redis, Blob)|
+-------------------------------------------------------+
```

### Core Principles
- **Dependency Inversion**: Dependencies point inwards. Domain code has zero external references.
- **Single Responsibility**: Each service owns its database and bounded context.
- **Fail-Fast & Graceful Degradation**: Using circuit breakers and fallback policies.

---

## 2. Implementing Resiliency with Polly

Transient network failures are inevitable in cloud environments. With Polly in .NET, you can easily configure retry mechanisms and circuit breakers:

```csharp
builder.Services.AddHttpClient(""PaymentServiceClient"", client =>
{
    client.BaseAddress = new Uri(""https://api.payments.internal/"");
    client.Timeout = TimeSpan.FromSeconds(5);
})
.AddStandardResilienceHandler(options =>
{
    options.Retry.MaxRetryAttempts = 3;
    options.Retry.Delay = TimeSpan.FromMilliseconds(200);
    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(10);
});
```

---

## 3. Asynchronous Messaging with Event-Driven Architecture

To decouple services, communicate via an asynchronous message broker (such as RabbitMQ, Kafka, or Azure Service Bus) using the **Transactional Outbox Pattern**.

> **Pro Tip**: Always ensure message consumers are idempotent to handle potential duplicate event deliveries safely.

---

## Summary & Next Steps

1. Structure services around business capabilities, not technical layers.
2. Implement distributed tracing with OpenTelemetry.
3. Protect APIs with rate-limiting and centralized API Gateways.
"
                },
                new
                {
                    Title = "Building Next-Generation User Interfaces with Modern CSS and Micro-Interactions",
                    Slug = "building-next-gen-ui-modern-css-micro-interactions",
                    Author = liam,
                    Status = "published",
                    ViewCount = 2310,
                    ReadingTimeMinutes = 5,
                    PublishedDaysAgo = 10,
                    CoverImage = "https://images.unsplash.com/photo-1507238691740-187a5b1d37b8?w=1200&auto=format&fit=crop&q=80",
                    Summary = "Explore cutting-edge CSS techniques including Subgrid, CSS Anchor Positioning, View Transitions API, and fluid glassmorphism design tokens.",
                    Categories = new[] { uiUxCat, webDevCat }.Where(c => c != null).Select(c => c!).ToList(),
                    Content = @"# Building Next-Generation User Interfaces with Modern CSS

Web styling has undergone a revolutionary shift. Modern browsers now natively support advanced layout paradigms and animation primitives that once required heavy JavaScript libraries.

Let's explore how to create delightful, high-performance interfaces with **Modern CSS** and **Micro-Interactions**.

---

## 1. Fluid Typography and Dynamic Color Tokens

Instead of declaring static breakpoints, use modern CSS mathematical functions like `clamp()` and `oklch()`:

```css
:root {
  --font-hero: clamp(2rem, 5vw + 1rem, 4.5rem);
  --color-primary: oklch(62% 0.24 265);
  --color-accent: oklch(75% 0.18 160);
  --glass-bg: oklch(20% 0.02 260 / 0.6);
  --glass-border: oklch(100% 0 0 / 0.12);
}

.glass-card {
  background: var(--glass-bg);
  backdrop-filter: blur(16px);
  border: 1px solid var(--glass-border);
  border-radius: 16px;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.37);
  transition: transform 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
}

.glass-card:hover {
  transform: translateY(-4px) scale(1.02);
}
```

---

## 2. Micro-Interactions that Delight Users

Micro-interactions bridge the gap between user intention and software response. They provide immediate, intuitive feedback:

* **Button Ripple/Press Feedback**: Subtle spring animations on click.
* **Skeleton Shimmers**: Progressive loading states with smooth wave animations.
* **Scroll-Driven Animations**: Native `animation-timeline: scroll()` for parallax and progress bars.

---

## 3. The View Transitions API

Multi-page and single-page apps can now animate shared layout elements seamlessly:

```javascript
document.startViewTransition(() => {
  updateDOMState();
});
```

Combined with CSS `view-transition-name`, components morph organically across routes!
"
                },
                new
                {
                    Title = "Deploying and Fine-Tuning Open Source LLMs in Enterprise Workflows",
                    Slug = "deploying-fine-tuning-open-source-llms",
                    Author = elena,
                    Status = "published",
                    ViewCount = 3420,
                    ReadingTimeMinutes = 9,
                    PublishedDaysAgo = 7,
                    CoverImage = "https://images.unsplash.com/photo-1620712943543-bcc4688e7485?w=1200&auto=format&fit=crop&q=80",
                    Summary = "A comprehensive deep dive into Quantization (GGUF/AWQ), vLLM deployment, Retrieval-Augmented Generation (RAG), and safety guardrails for enterprise AI systems.",
                    Categories = new[] { aiCat, cloudCat }.Where(c => c != null).Select(c => c!).ToList(),
                    Content = @"# Deploying and Fine-Tuning Open Source LLMs in Enterprise Workflows

As open-weight foundation models like Llama 3, Mistral, and DeepSeek continue to improve, enterprises are choosing self-hosted AI stacks to protect intellectual property and minimize API inference costs.

This article provides a practical blueprint for fine-tuning, serving, and integrating private LLMs.

---

## 1. Parameter-Efficient Fine-Tuning (PEFT / QLoRA)

Training an entire 70B parameter model from scratch is cost-prohibitive. QLoRA (Quantized Low-Rank Adaptation) reduces memory overhead by 80% while preserving benchmark accuracy.

### Key Benefits of QLoRA:
1. Freezes base model weights in 4-bit NormalFloat precision.
2. Attaches lightweight adapter matrices to attention layers.
3. Enables fine-tuning on consumer/standard enterprise GPUs (e.g., A10G or L4).

---

## 2. Ultra-Fast Inference with vLLM & PagedAttention

Traditional transformers suffer from memory fragmentation in the KV Cache. **vLLM** solves this with PagedAttention, achieving up to 10x higher serving throughput.

```bash
# Launch vLLM server with OpenAI-compatible API
vllm serve meta-llama/Meta-Llama-3-8B-Instruct \
  --port 8000 \
  --gpu-memory-utilization 0.90 \
  --max-model-len 8192
```

---

## 3. Production RAG Architecture

```
[User Query] 
     │
     ▼
[Embedding Model] ──► [Vector DB (Qdrant / Milvus)]
                             │
                             ▼ (Relevant Chunks)
[Reranker] ──► [Context Window + System Prompt] ──► [LLM Generator] ──► [Streamed Output]
```

### Best Practices:
- Hybrid Search (Dense Embeddings + BM25 Sparse Search).
- Cohere/BGE Cross-Encoder Reranking for context precision.
- Hallucination detection guardrails before streaming responses to clients.
"
                },
                new
                {
                    Title = "Mastering State Management in Flutter: Riverpod vs Bloc in 2026",
                    Slug = "mastering-state-management-flutter-riverpod-bloc",
                    Author = liam,
                    Status = "published",
                    ViewCount = 980,
                    ReadingTimeMinutes = 6,
                    PublishedDaysAgo = 4,
                    CoverImage = "https://images.unsplash.com/photo-1551650975-87deedd944c3?w=1200&auto=format&fit=crop&q=80",
                    Summary = "An in-depth comparison between Riverpod 2.x and BLoC pattern for large scale enterprise mobile apps with real-world architectural examples.",
                    Categories = new[] { mobileCat }.Where(c => c != null).Select(c => c!).ToList(),
                    Content = @"# Mastering State Management in Flutter: Riverpod vs Bloc

Choosing the right state management strategy in Flutter is crucial for code maintainability, testability, and team scalability.

Here is an architectural comparison of **Riverpod** and **BLoC (Business Logic Component)**.

---

## Feature Comparison

| Criteria | Riverpod 2.x | BLoC Pattern |
| :--- | :--- | :--- |
| **Boilerplate** | Low (with code generation) | Moderate to High |
| **Learning Curve** | Gentle to Moderate | Steeper |
| **Testability** | Outstanding (override providers) | Outstanding (stream testing) |
| **Async Handling** | Built-in `AsyncValue` | Custom state unions |
| **Community Support** | Rapidly Growing | Extremely Established |

---

## Riverpod with Code Generation Example

```dart
@riverpod
class BlogListNotifier extends _$BlogListNotifier {
  @override
  Future<List<Blog>> build() async {
    return ref.read(blogRepositoryProvider).fetchTrendingBlogs();
  }

  Future<void> refresh() async {
    state = const AsyncValue.loading();
    state = await AsyncValue.guard(() => ref.read(blogRepositoryProvider).fetchTrendingBlogs());
  }
}
```

---

## Verdict: Which Should You Choose?

- Choose **Riverpod** if you want declarative reactivity, minimal boilerplate, automatic dependency caching, and compile-safe provider access.
- Choose **BLoC** if your team follows strict event-driven architecture, event audits, and wants standardized state transitions across large enterprise teams.
"
                },
                new
                {
                    Title = "The Senior Engineer's Guide to System Design & Technical Leadership",
                    Slug = "senior-engineers-guide-system-design-leadership",
                    Author = sophia,
                    Status = "published",
                    ViewCount = 4120,
                    ReadingTimeMinutes = 8,
                    PublishedDaysAgo = 3,
                    CoverImage = "https://images.unsplash.com/photo-1531403009284-440f080d1e12?w=1200&auto=format&fit=crop&q=80",
                    Summary = "Key principles for leading cross-functional teams, writing RFCs, managing technical debt, and designing distributed systems that scale sustainably.",
                    Categories = new[] { careerCat, webDevCat }.Where(c => c != null).Select(c => c!).ToList(),
                    Content = @"# The Senior Engineer's Guide to System Design & Technical Leadership

Transitioning from Mid-level to Senior and Staff Engineer is less about writing more lines of code and more about technical leverage, clear communication, and architectural judgment.

---

## 1. Writing Effective RFCs (Request for Comments)

Before writing code for complex systems, write an RFC. An RFC aligns stakeholders, uncovers blind spots, and documents architectural trade-offs.

### Key Sections of an RFC:
1. **Summary & Problem Statement**: What problem are we solving, and why now?
2. **Non-Goals**: What are we explicitly NOT solving in this phase?
3. **Proposed Architecture**: Diagrams, data flow, APIs, database schemas.
4. **Alternatives Considered**: Why did we reject option A or B?
5. **Rollout, Observability & Rollback Plan**: How will we safely deploy and monitor?

---

## 2. Managing Technical Debt Strategically

Technical debt is not always bad; it is leverage. Like financial debt, deliberate technical debt taken to meet a critical market window can be justified—provided interest is paid down systematically.

* **Allocate 20% of every sprint** to refactoring, tooling improvements, and dependency upgrades.
* **Maintain an Architectural Decision Record (ADR)** repository to track historical reasoning.

---

## 3. Designing for Failure

> ""Everything fails, all the time."" — Werner Vogels

When designing systems:
- Assume network partitions will happen.
- Protect downstreams with bulkhead isolation and exponential backoff.
- Implement comprehensive health checks, metrics, and alerting dashboards.
"
                },
                new
                {
                    Title = "Understanding Web Security: Defending Against CSRF, XSS, and Injection in Modern SPAs",
                    Slug = "understanding-web-security-csrf-xss-injection-spas",
                    Author = sophia,
                    Status = "published",
                    ViewCount = 1560,
                    ReadingTimeMinutes = 6,
                    PublishedDaysAgo = 1,
                    CoverImage = "https://images.unsplash.com/photo-1563986768609-322da13575f3?w=1200&auto=format&fit=crop&q=80",
                    Summary = "A comprehensive security checklist for modern single-page applications and REST APIs covering JWT storage, Content Security Policy, and sanitized inputs.",
                    Categories = new[] { webDevCat }.Where(c => c != null).Select(c => c!).ToList(),
                    Content = @"# Understanding Web Security: Defending Against CSRF, XSS, and Injection in Modern SPAs

Security should never be an afterthought. In modern web architectures combining Single Page Applications (React, Vue, Next.js) with ASP.NET Core REST backends, understanding the attack surface is essential.

---

## 1. JWT Storage & XSS Mitigation

Storing JWT access tokens in `localStorage` makes them vulnerable to Cross-Site Scripting (XSS) attacks.

### Recommended Approaches:
- **HttpOnly, Secure, SameSite=Strict Cookies**: Prevents JavaScript from reading the session tokens.
- **Short-Lived Memory Tokens**: Store the Access Token in JavaScript memory and use an HttpOnly Refresh Token cookie to rotate tokens.

---

## 2. Strict Content Security Policy (CSP)

Implement strict CSP headers to restrict where scripts, styles, and media can be loaded from:

```http
Content-Security-Policy: default-src 'self'; script-src 'self' 'nonce-rAnd0m'; style-src 'self' 'unsafe-inline'; img-src 'self' https: data:;
```

---

## 3. Parameterized Queries & ORM Protections

Always use parameterized queries or Entity Framework Core LINQ queries to eliminate SQL Injection vulnerabilities completely.
"
                },
                new
                {
                    Title = "Draft: Deep Dive into Quantum Computing Algorithms and Qubit Gates",
                    Slug = "draft-deep-dive-quantum-computing-algorithms",
                    Author = elena,
                    Status = "draft",
                    ViewCount = 0,
                    ReadingTimeMinutes = 4,
                    PublishedDaysAgo = 0,
                    CoverImage = "https://images.unsplash.com/photo-1639762681485-074b7f938ba0?w=1200&auto=format&fit=crop&q=80",
                    Summary = "An introduction to quantum superposition, entanglement, Hadamard gates, and Grover's search algorithm.",
                    Categories = new[] { aiCat }.Where(c => c != null).Select(c => c!).ToList(),
                    Content = @"# Draft: Deep Dive into Quantum Computing Algorithms

Quantum computing harnesses quantum mechanical phenomena like superposition and entanglement to perform computations exponentially faster than classical supercomputers for specific problem spaces.

* Work in progress draft notes.
* Equations for Shor's and Grover's algorithms to be added.
"
                }
            };

            foreach (var item in blogData)
            {
                var existingBlog = await context.Blogs
                    .Include(b => b.BlogCategories)
                    .Include(b => b.Media)
                    .FirstOrDefaultAsync(b => b.Slug == item.Slug);

                if (existingBlog == null)
                {
                    var blog = new Blog
                    {
                        Id = Guid.NewGuid(),
                        Title = item.Title,
                        Slug = item.Slug,
                        Summary = item.Summary,
                        Content = item.Content,
                        Status = item.Status,
                        ViewCount = item.ViewCount,
                        ReadingTimeMinutes = item.ReadingTimeMinutes,
                        AuthorId = item.Author.Id,
                        PublishedAt = item.Status == "published" ? DateTime.UtcNow.AddDays(-item.PublishedDaysAgo) : null,
                        CreatedAt = DateTime.UtcNow.AddDays(-item.PublishedDaysAgo - 1),
                        SeoJson = $"{{\"keywords\":[\"{string.Join("\", \"", item.Categories.Select(c => c.Name))}\"],\"metaTitle\":\"{item.Title}\"}}"
                    };

                    await context.Blogs.AddAsync(blog);

                    // Add cover media
                    var media = new BlogHasMedia
                    {
                        Id = Guid.NewGuid(),
                        BlogId = blog.Id,
                        FilePath = item.CoverImage,
                        FileName = Path.GetFileName(new Uri(item.CoverImage).AbsolutePath),
                        MimeType = "image/jpeg",
                        FileSize = 250000,
                        DisplayOrder = 0,
                        IsPrimary = true,
                        CreatedAt = blog.CreatedAt
                    };
                    await context.BlogHasMedia.AddAsync(media);

                    // Add categories
                    foreach (var cat in item.Categories)
                    {
                        var blogCat = new BlogHasCategory
                        {
                            BlogId = blog.Id,
                            CategoryId = cat.Id
                        };
                        await context.BlogCategories.AddAsync(blogCat);
                    }

                    seededBlogs.Add(blog);
                }
                else
                {
                    seededBlogs.Add(existingBlog);
                }
            }

            await context.SaveChangesAsync();
            return seededBlogs;
        }
    }
}
